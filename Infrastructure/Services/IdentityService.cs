using Application.Accounts.Commands;
using Application.Accounts.Queries;
using Application.Common.Exceptions;
using Application.Common.Extensions;
using Application.Common.Interfaces;
using Application.Common.Models;
using Domain.Common;
using Domain.Contracts;
using Domain.Entities;
using Domain.Enum;
using Infrastructure.Identity;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using System.Linq.Dynamic.Core;
using NetTopologySuite;
using NetTopologySuite.Geometries;
using System.Globalization;
using Domain.Generics;
using Application.Services.Queries;
using Domain.Helpers;
using Stripe;
using Stripe.Checkout;

namespace Infrastructure.Services;

public class IdentityService : IIdentityService
{
    private readonly UserManager<ApplicationUser> _userManager;
    private readonly ITokenProvider _jwtToken;
    private readonly ICurrentUserService _currentUser;
    private readonly IDateTime _dateTime;
    private readonly IApplicationDbContext _context;
    private readonly SignInManager<ApplicationUser> _signInManager;
    private readonly RoleManager<IdentityRole> _roleManager;
    private readonly IEmailService _emailService;
    public IFormatProvider provider { get; private set; }


    public IdentityService(UserManager<ApplicationUser> userManager, SignInManager<ApplicationUser> signInManager, ITokenProvider jwtToken, IDateTime dateTime, IApplicationDbContext context, ICurrentUserService currentUser, RoleManager<IdentityRole> roleManager, IEmailService emailService)
    {
        _userManager = userManager;
        _signInManager = signInManager;
        _jwtToken = jwtToken;
        _dateTime = dateTime;
        _context = context;
        _currentUser = currentUser;
        _roleManager = roleManager;
        _emailService = emailService;
    }


    /// <summary>
    /// Comands
    /// </summary>
    /// <param name="request"></param>
    /// <returns></returns>

    public async Task<bool> AddTestingUserData(ttt request, CancellationToken token)
    {


        AppContent data1 = AppContent.Create(request.profilePic.ToString(), request.name.ToString(), request.email.ToString(), AppContentTypeEnum.ProfileNameEmail, DateTime.UtcNow);
        AppContent data2 = AppContent.Create(request.password.ToString(), request.phoneNumber.ToString(), request.dateOfBirth.ToString(), AppContentTypeEnum.PasswordPhoneDob, DateTime.UtcNow);
        AppContent data3 = AppContent.Create(request.gender.ToString(), request.level.ToString(), request.playingTennis.ToString(), AppContentTypeEnum.GenderLevelPlayTennis, DateTime.UtcNow);
        AppContent data4 = AppContent.Create(request.playInMonth.ToString(), request.clubName.ToString(), request.latitute.ToString(), AppContentTypeEnum.PlayInMonthtClubLatitue, DateTime.UtcNow);
        AppContent data5 = AppContent.Create(request.longitute.ToString(), request.address.ToString(), request.radius.ToString(), AppContentTypeEnum.LongituteAddressRadius, DateTime.UtcNow);

        await _context.AppContent.AddAsync(data1);
        await _context.AppContent.AddAsync(data2);
        await _context.AppContent.AddAsync(data3);
        await _context.AppContent.AddAsync(data4);
        await _context.AppContent.AddAsync(data5);
        await _context.SaveChangesAsync(token);
        return true;
    }


    public async Task<(Result Result, string UserId)> CreateAccountAsync(CreateAccountCommand request, CancellationToken cancellationToken)
    {
        DateTime dateOfBirth;
        DateTime dateTime10; // 1/1/0001 12:00:00 AM  
        bool isSuccess = DateTime.TryParseExact(request.dateOfBirth, new string[] { "MM.dd.yyyy", "MM-dd-yyyy", "MM/dd/yyyy" }, provider, DateTimeStyles.None, out dateTime10);
        if (isSuccess)
            dateOfBirth = DateTime.ParseExact(request.dateOfBirth, new string[] { "MM.dd.yyyy", "MM-dd-yyyy", "MM/dd/yyyy" }, provider, DateTimeStyles.None);
        else
            throw new CustomInvalidOperationException("MM.dd.yyyy, MM-dd-yyyy, MM/dd/yyyy these formats are supported only!");


        var geometryFactory = NtsGeometryServices.Instance.CreateGeometryFactory(srid: 4326);
        Point userLocation = geometryFactory.CreatePoint(new Coordinate(request.longitute, request.latitute));

        //Get the league
        var x1 = GetCategoryPoints(1, request.level);
        var x2 = GetCategoryPoints(2, request.playingTennis);
        var x3 = GetCategoryPoints(3, request.playInMonth);
        var x4 = GetCategoryPoints(4, request.dtbPerformanceClass);

        var recommendedPoints = (300 + ((x1 / 6) * 50) + ((x2 / 6) * 50) + ((x3 / 3) * 100) + ((x4 / 6) * 200));

        var league = _context.SubLeagues.Where(x => x.MinRange <= recommendedPoints).OrderByDescending(x => x.MinRange).FirstOrDefault();
        if (league == null)
            throw new NotFoundException("League not found");

        var user = new ApplicationUser
        {
            ProfileImageUrl = request.profilePic,
            Fullname = request.name.Trim(),
            UserName = request.email.ToLower().Trim(),
            Email = request.email.ToLower().Trim(),
            LoginRole = UserTypeEnum.User,
            CreatedAt = _dateTime.NowUTC,
            AccountStatus = StatusEnum.Active,
            Gender = (GenderTypeEnum)request.gender,
            DateOfBrith = dateOfBirth,
            DtbPerformanceClass = (DTBEnum)request.dtbPerformanceClass,
            Address = request.address,
            ClubName = request.clubName,
            EmailConfirmed = false,
            isSubscriptionPurchased = false,
            Level = (LevelEnum)request.level,
            Location = userLocation,
            MonthPlayTime = (MonthPlayTimeEnum)request.playInMonth,
            PhoneNumber = request.phoneNumber,
            PlayingTennis = (PlayingTimeEnum)request.playingTennis,
            Points = recommendedPoints,
            Radius = request.radius,
            Rating = 0,
            ReviewPersonCount = 0,
            RequestForPermanentDelete = false,
        };

        var result = await _userManager.CreateAsync(user, request.password);
        if (result.Succeeded)
        {
            //Assigning freemium package

            var assignedPackage = _context.Subscriptions.Where(x => x.SubscriptionType == SubscriptionTypeEnum.FreemiumModel).FirstOrDefault();
            if (assignedPackage == null)
                throw new NotFoundException("Package not found");

            //Set Package Expiry
            var expiry = assignedPackage.DurationType == DurationTypeEnum.Month ? DateTime.UtcNow.AddMonths(1) :
                      assignedPackage.DurationType == DurationTypeEnum.Day ? DateTime.UtcNow.AddDays(1) :
                      assignedPackage.DurationType == DurationTypeEnum.Year ? DateTime.UtcNow.AddYears(1)
                      : assignedPackage.DurationType == DurationTypeEnum.Week ? DateTime.UtcNow.AddDays(7) : DateTime.UtcNow;

            expiry = new DateTime(expiry.Year, expiry.Month, expiry.Day, 23, 59, 59, 999);

            _context.SubscriptionHistories.Add(SubscriptionHistory.Create(user.Id, assignedPackage.Id,
                assignedPackage.Price, assignedPackage.PriceAfterDiscount, false, assignedPackage.Discount,
                assignedPackage.CostPerRankedGame, assignedPackage.CostPerUnrankedGame,
                assignedPackage.IsFreeRankedUnlimited, assignedPackage.IsFreeUnrankedUnlimited,
                assignedPackage.FreeRankedGames, assignedPackage.FreeUnrankedGames,
                assignedPackage.FreeRankedGames, assignedPackage.FreeUnrankedGames,
                assignedPackage.IsPaidCouchingContentAvailable, assignedPackage.IsFreeCouchingContentAvailable,
                assignedPackage.IsRatingAvailable, assignedPackage.IsMatchBalanceAvailable, assignedPackage.IsScoreAvailable, assignedPackage.IsReviewsAvailable,
                expiry, 0, null, "", UserPackageStatusEnum.Active, _dateTime.NowUTC));


            //user.IsFreeRankedUnlimited = assignedPackage.IsFreeRankedUnlimited;
            //user.IsFreeUnrankedUnlimited = assignedPackage.IsFreeUnrankedUnlimited;
            //user.RemainingFreeRankedMatches = assignedPackage.FreeRankedGames;
            //user.RemainingFreeUnrankedMatches = assignedPackage.FreeUnrankedGames;

            //user.AwardedFreeRankedMatches = assignedPackage.FreeRankedGames;
            //user.AwardedFreeUnrankedMatches = assignedPackage.FreeUnrankedGames;

            //user.IsFreeCouchingContentAvailable = assignedPackage.IsFreeCouchingContentAvailable;
            //user.IsPaidCouchingContentAvailable = assignedPackage.IsPaidCouchingContentAvailable;

            //user.IsStatsAvailable = assignedPackage.IsStatsAvailable;
            //user.IsScoreAvailable = assignedPackage.IsScoreAvailable;
            //user.IsReviewsAvailable = assignedPackage.IsReviewsAvailable;

            //await _userManager.UpdateAsync(user);
            //Create user setting table
            await _context.UserSettings.AddAsync(UserSetting.Create(user.Id, league.Id, null));
            await _context.SaveChangesAsync(cancellationToken);
        }

        return (result.ToApplicationResult(), user.Id);
    }
    public async Task<Result> SendEmailOTPAsync(SendEmailOtpCommand request, CancellationToken token)
    {
        //Init application user entity to null
        if (!request.newEmail.Equals("#"))
        {
            var IsUnique = await BeUniqueEmailAsync(request.newEmail.Trim(), token);
            if (!IsUnique)
                throw new NotFoundException(InvalidOperationErrorMessage.AlreadyExistsErrorMessage("User", "replacement email"));
        }

        //Fetch user info
        var user = await _userManager.Users.FirstOrDefaultAsync(u => u.Email == request.existingEmail.Trim(), token);
        if (user == null)
            throw new NotFoundException($"User with {request.existingEmail.Trim()} does not exist");


        //Remove all previous otp code belongs to this user
        _context.AccountOtpHistory.RemoveRange(await _context.AccountOtpHistory.Where(u => u.CreatedBy == user.Id).ToListAsync(token));

        Random rnd = new Random();
        var otp = rnd.Next(111111, 999999);

        //Send otp to email
        _emailService.SendEmail(request.existingEmail, otp);

        var expiryDate = _dateTime.NowUTC;
        AccountOtpHistory accountOtpCode = AccountOtpHistory.Create(otp.ToString(), expiryDate.AddDays(1), OtpMediaTypeEnum.email, request.newEmail, request.existingEmail, user.Id);

        await _context.AccountOtpHistory.AddAsync(accountOtpCode);
        await _context.SaveChangesAsync(token);
        return Result.Success();
    }
    public async Task<ResponseKeyContract> ConfirmEmailAsync(ConfirmEmailCommand info, CancellationToken cancellationToken)
    {
        //Check if user does'nt exists
        var user = await _userManager.FindByEmailAsync(info.email.Trim());
        if (user == null)
            throw new NotFoundException($"User with {info.email.Trim()} does not exist");

        //Check if otp does'nt exist
        var userOtp = await _context.AccountOtpHistory.FirstOrDefaultAsync(u => u.CreatedBy == user.Id && u.OtpMediaType == OtpMediaTypeEnum.email && u.Code == info.otp.Trim(), cancellationToken);
        if (userOtp == null)
            throw new CustomInvalidOperationException(InvalidOperationErrorMessage.otp_code_not_found);

        //Check if otp is expired
        if (_dateTime.NowUTC > userOtp.ExpiryDateTime)
            throw new CustomInvalidOperationException(InvalidOperationErrorMessage.otp_code_has_expired);

        string newEmail = "";

        if (!userOtp.ReplacementValue.Equals("#"))
        {
            //Check if email already exist or not
            var IsUnique = await BeUniqueEmailAsync(userOtp.ReplacementValue, cancellationToken);
            if (!IsUnique)
                throw new CustomInvalidOperationException(InvalidOperationErrorMessage.AlreadyTakenErrorMessage("Email", userOtp.ReplacementValue));
            newEmail = userOtp.ReplacementValue;
        }
        else
        {
            newEmail = user.Email;
        }
        //Change email
        var validToken = await _userManager.GenerateChangeEmailTokenAsync(user, newEmail);
        var result = await _userManager.ChangeEmailAsync(user, newEmail, validToken);
        //Remove all previous otp code belongs to this user
        if (result.Succeeded)
        {
            _context.AccountOtpHistory.RemoveRange(await _context.AccountOtpHistory.Where(u => u.CreatedBy == user.Id).ToListAsync(cancellationToken));
            await _context.SaveChangesAsync(cancellationToken);
        }
        else
        {
            throw new CustomInvalidOperationException(result.Errors.ToString() ?? "email not confirmed");
        }
        //Create Jwt token for Authenticate
        DateTime expiry = DateTime.UtcNow.AddDays(20);
        var accessToken = _jwtToken.CreateToken(new JwtUserContract { email = user.Email, id = user.Id, userName = user.UserName, loginRole = (int)user.LoginRole }, expiry);

        //Return User Contract
        return new ResponseKeyContract
        {
            key = accessToken,
            emailConfirmationRequired = user.EmailConfirmed == false ? true : false,
        };
    }
    public async Task<(Result Result, string UserId)> ResetPasswordViaEmailAsync(ResetPasswordViaEmailCommand request)
    {
        ApplicationUser user = await _userManager.Users.FirstOrDefaultAsync(u => u.Email == request.email.Trim());

        //Check if user does'nt exists
        if (user == null)
            throw new NotFoundException($"User with {request.email.Trim()} does not exist");

        //Authentication
        if (request.resetOption == (int)ResetPasswordOptionEnum.Otp)
        {
            //Check if otp does'nt exist
            var userOtp = await _context.AccountOtpHistory.FirstOrDefaultAsync(u => u.CreatedBy == user.Id && u.OtpMediaType == OtpMediaTypeEnum.email && u.Code == request.resetValue.Trim());
            if (userOtp == null)
                throw new CustomInvalidOperationException(InvalidOperationErrorMessage.otp_code_not_found);

            //Check if otp is expired
            if (_dateTime.NowUTC > userOtp.ExpiryDateTime)
                throw new CustomInvalidOperationException(InvalidOperationErrorMessage.otp_code_has_expired);
        }
        else
        {
            var isPasswordValid = await _userManager.CheckPasswordAsync(user, request.resetValue);
            if (!isPasswordValid)
                throw new CustomInvalidOperationException(InvalidOperationErrorMessage.invalid_password);
        }


        //Change password
        var token = await _userManager.GeneratePasswordResetTokenAsync(user);
        var resetPassResult = await _userManager.ResetPasswordAsync(user, token, request.password);
        //Return Result
        return (resetPassResult.ToApplicationResult(), user.Id);
    }
    public async Task<Result> RequestForAccountDeleteAsync(string password)
    {
        // Get the existing user from the database
        var user = await _userManager.FindByIdAsync(_currentUser.UserId);

        //thrown error if user not found
        if (user == null)
            throw new NotFoundException("user does not exist");

        var isPasswordValid = await _userManager.CheckPasswordAsync(user, password);
        if (!isPasswordValid)
            throw new CustomInvalidOperationException(InvalidOperationErrorMessage.invalid_password);

        user.DeleteAccountRequestedAt = _dateTime.NowUTC;
        user.RequestForPermanentDelete = true;

        // Apply the changes if any to the db
        var result = await _userManager.UpdateAsync(user);

        return result.ToApplicationResult();
    }
    public async Task<Result> RemoveProfileImageAsync()
    {
        // Get the existing user from the database
        var user = await _userManager.FindByIdAsync(_currentUser.UserId);

        //thrown error if user not found
        if (user == null)
            throw new NotFoundException("user does not exist");

        user.ProfileImageUrl = "";

        // Apply the changes if any to the db
        var result = await _userManager.UpdateAsync(user);

        return result.ToApplicationResult();
    }
    public async Task<Result> ResetPasswordAsync(string password)
    {
        var user = await _userManager.Users.FirstOrDefaultAsync(u => u.Id == _currentUser.UserId);

        //Check if user does'nt exists
        if (user == null)
            throw new NotFoundException($"User does not exist");

        //Change password
        var token = await _userManager.GeneratePasswordResetTokenAsync(user);
        var resetPassResult = await _userManager.ResetPasswordAsync(user, token, password);

        //Return Result
        return resetPassResult.ToApplicationResult();
    }
    public async Task<Result> UpdateAccountDetailAsync(UpdateProfileCommand request, CancellationToken token)
    {
        // Get the existing student from the db
        var user = await _userManager.FindByIdAsync(_currentUser.UserId);
        if (user == null)
            throw new NotFoundException("User not found");

        // Update it with the values from the view model

        if (!string.IsNullOrEmpty(request.name))
            user.Fullname = request.name;

        if (!string.IsNullOrEmpty(request.profileImageUrl))
            user.ProfileImageUrl = request.profileImageUrl;


        if (!string.IsNullOrEmpty(request.phone))
        {
            var existingPhone = await BeUniquePhoneAsync(request.phone, token);
            if (existingPhone)
                user.PhoneNumber = request.phone;
        }


        user.ClubName = request.clubName;

        if (!string.IsNullOrEmpty(request.about))
            user.About = request.about;

        if (request.latitute.HasValue && request.longitute.HasValue)
        {
            var geometryFactory = NtsGeometryServices.Instance.CreateGeometryFactory(srid: 4326);
            Point userLocation = geometryFactory.CreatePoint(new Coordinate((double)request.longitute, (double)request.latitute));

            user.Address = request.address;
            user.Location = userLocation;
        }

        if (request.radius.HasValue)
            user.Radius = (double)request.radius;

        //Previous interests
        var userInterests = _context.UserInterests.Where(x => x.CreatedBy == user.Id).ToList();
        if (userInterests != null && userInterests.Count > 0)
        {
            foreach (var item in userInterests)
            {
                _context.UserInterests.Remove(item);
            }
        }

        //Add new interests
        if (request.interests != null && request.interests.Count > 0)
        {
            foreach (var item in request.interests)
            {
                _context.UserInterests.Add(UserInterest.Create(user.Id, item, _dateTime.NowUTC));
            }
        }

        await _context.SaveChangesAsync(token);

        // Apply the changes if any to the db
        var result = await _userManager.UpdateAsync(user);
        return result.ToApplicationResult();
    }
    public async Task<Result> UpdateUserPackageDetailAsync(string userId)
    {
        var user = await _userManager.Users.Where(x => x.Id == userId).FirstOrDefaultAsync();
        if (user == null)
            throw new NotFoundException("User does not exist");

        user.isSubscriptionPurchased = true;

        // Apply the changes if any to the db
        var result = await _userManager.UpdateAsync(user);
        return result.ToApplicationResult();
    }
    public async Task<Result> UpdateUserPointAsync(string userId, double point)
    {
        var user = await _userManager.Users.Where(x => x.Id == userId).FirstOrDefaultAsync();
        if (user == null)
            throw new NotFoundException("User does not exist");

        user.Points = point;

        // Apply the changes if any to the db
        var result = await _userManager.UpdateAsync(user);
        return result.ToApplicationResult();
    }

    public async Task<Result> LogoutAsync()
    {
        // Get the existing user from the database
        var user = await _userManager.FindByIdAsync(_currentUser.UserId);

        //thrown error if user not found
        if (user == null)
            throw new NotFoundException("user does not exist");

        user.fcmToken = null;

        // Apply the changes if any to the db
        var result = await _userManager.UpdateAsync(user);
        return result.ToApplicationResult();
    }
    public async Task<Result> UpdateUserStatusAsync(StatusUpdateCommand request)
    {
        ApplicationUser user = new ApplicationUser();
        //User status update
        switch (request.isDeleted)
        {
            case 1:
                user = await _userManager.Users.Where(x => x.Id == request.id).FirstOrDefaultAsync();

                //Update user status
                user.BlockReason = request.reason;
                user.AccountStatus = StatusEnum.Blocked;
                await _userManager.UpdateAsync(user);

                break;

            case 2:

                user = await _userManager.Users.Where(x => x.Id == request.id).FirstOrDefaultAsync();

                //Update user status
                user.AccountStatus = StatusEnum.Active;
                await _userManager.UpdateAsync(user);

                break;

            default:
                break;
        }

        var result = await _userManager.UpdateAsync(user);
        return result.ToApplicationResult();

    }

    public async Task<Result> UpdateRatingAsync(string userId, double rating, int reviewBy)
    {
        // Get the existing student from the db
        var user = await _userManager.FindByIdAsync(userId);
        if (user == null)
            throw new NotFoundException("User not found");

        user.Rating = rating;
        user.ReviewPersonCount = reviewBy;

        var result = await _userManager.UpdateAsync(user);
        return result.ToApplicationResult();
    }

    /// <summary>
    /// Queries
    /// </summary>
    /// <param name="request"></param>
    /// <returns></returns>
    /// <exception cref="NotFoundException"></exception>
    /// <exception cref="CustomInvalidOperationException"></exception>
    /// 

    //Get Match Creators Detail
    public async Task<DatatableResponse<List<GenericPostQueryListModel<MatchOwnerDetailContract>>>> GetMatchCreatorDetailAsync(DataTablePaginationFilter validFilter, CancellationToken token)
    {
        //Current login user detail
        var user = await _userManager.Users.Where(x => x.Id == _currentUser.UserId).FirstOrDefaultAsync();

        IQueryable<IGrouping<string, TennisMatch>> resultQuery = _context.TennisMatches.Where(x => x.Deleted == false).GroupBy(x => x.CreatedBy).AsQueryable();

        IQueryable<GenericPreQueryListModel<MatchOwnerDetailContract>> query = resultQuery.Select(u => new GenericPreQueryListModel<MatchOwnerDetailContract>
        {
            createdAt = u.Select(x => x.Created).FirstOrDefault(),
            lastUpdatedAt = u.Select(x => x.Modified).FirstOrDefault(),
            createdBy = u.Select(x => x.CreatedBy).FirstOrDefault(),
            updatedBy = u.Select(x => x.ModifiedBy).FirstOrDefault(),
            data = new MatchOwnerDetailContract
            {
                totalMatches = _context.TennisMatches.Where(x => x.CreatedBy == u.Key).Count(),
                id = u.Key,
                profilePic = _userManager.Users.Where(x => x.Id == u.Key).Select(x => x.ProfileImageUrl).FirstOrDefault() ?? "N/A",
                name = _userManager.Users.Where(x => x.Id == u.Key).Select(x => x.Fullname).FirstOrDefault() ?? "N/A",
                email = _userManager.Users.Where(x => x.Id == u.Key).Select(x => x.Email).FirstOrDefault(),
                phone = _userManager.Users.Where(x => x.Id == u.Key).Select(x => x.PhoneNumber ?? "N/A").FirstOrDefault(),
            }
        }).AsQueryable();


        //Searching
        if (!string.IsNullOrEmpty(validFilter.searchValue))
        {
            string? searchString = validFilter?.searchValue.Trim();
            query = query.Where(u =>
                        u.data.name.Contains(searchString) || u.data.email.Contains(searchString));
        }

        //Sorting
        if (!string.IsNullOrEmpty(validFilter.sortColumn) && !string.IsNullOrEmpty(validFilter.sortColumnDirection))
        {
            query = query.OrderBy(validFilter.sortColumn + " " + validFilter.sortColumnDirection);
        }

        List<GenericPostQueryListModel<MatchOwnerDetailContract>> pagedData = await query.Select(u => new GenericPostQueryListModel<MatchOwnerDetailContract>
        {
            createdAt = u.createdAt.Value.ToString(_dateTime.shortDateFormat),
            lastUpdatedAt = u.lastUpdatedAt.HasValue ? u.lastUpdatedAt.Value.ToString(_dateTime.shortDateFormat) : "N/A",
            createdBy = new LeagueRankingUserContract(),
            updatedBy = _userManager.Users.Where(x => x.Id == u.updatedBy).Select(x => x.UserName).FirstOrDefault(),
            data = new MatchOwnerDetailContract
            {
                totalMatches = u.data.totalMatches,
                id = u.data.id,
                name = u.data.name,
                email = u.data.email,
                phone = u.data.phone,
                profilePic = u.data.profilePic,
            }
        }).Skip(validFilter.pageNumber ?? 0).Take(validFilter.pageSize ?? 0)
            .ToListAsync(token);

        var totalRecords = await query.CountAsync(token);
        var pagedReponse = PaginationHelper.CreateDatatableReponse(pagedData, validFilter, totalRecords);
        return pagedReponse;
    }

    public async Task<ResponseKeyContract> AuthenticateUserAsync(LoginQuery request)
    {
        //Get User Detail From database and check user exist 
        var user = await _userManager.Users.FirstOrDefaultAsync(u => u.Email == request.email.Trim());
        if (user == null)
            throw new NotFoundException($"User with {request.email.Trim()} does not exist");

        //Checking if account deleted
        if (user.RequestForPermanentDelete == true)
            throw new NotFoundException("Error! account is in deletion process.");


        //Checking Password
        if (!(await _userManager.CheckPasswordAsync(user, request.password)))
            throw new NotFoundException("Invalid password! try again with new password.");

        //Check Account Status
        if (user.AccountStatus != StatusEnum.Active)
            throw new CustomInvalidOperationException("Your account has been blocked");

        var result = await _signInManager.PasswordSignInAsync(user, request.password, true, false);

        if (result.IsLockedOut)
            throw new CustomInvalidOperationException("This account is locked out");

        user.TimezoneId = request.timeZoneId;
        user.fcmToken = request.fcmToken;

        await _userManager.UpdateAsync(user);
        //Create Jwt token for Authenticate
        DateTime expiry = DateTime.UtcNow.AddDays(20);
        var accessToken = _jwtToken.CreateToken(new JwtUserContract { email = user.Email, id = user.Id, userName = user.UserName, loginRole = (int)user.LoginRole }, expiry);

        //Return User Contract
        return new ResponseKeyContract
        {
            key = accessToken,
            emailConfirmationRequired = user.EmailConfirmed == false ? true : false,
        };
    }
    public async Task<bool> BeUniqueEmailAsync(string email, CancellationToken token)
    {
        var emailExist = await _userManager.Users.Where(x => x.Email == email).FirstOrDefaultAsync();
        if (emailExist != null)
        {
            if (emailExist.RequestForPermanentDelete == true)
                throw new NotFoundException("Error! account is in deletion process.");
            else
                return false;
        }

        return true;
    }
    public async Task<bool> BeUniquePhoneAsync(string phone, CancellationToken token)
    {
        var phoneNumberExist = await _userManager.Users.AnyAsync(u => u.PhoneNumber == phone.Trim(), token);
        if (phoneNumberExist)
            return false;
        return true;
    }

    public async Task<SubscriptionHistory> GetPackageDetailAsync(string userId, GameTypeEnum type)
    {
        SubscriptionHistory purchasedPck = null;
        var isPurchasedPackFound = false;

        var subscriptionPurchased = _context.SubscriptionHistories.Include(x => x.Subscription)
            .Where(p => p.CreatedBy == userId &&
            (p.SubscriptionStatus == UserPackageStatusEnum.TempActive || p.SubscriptionStatus == UserPackageStatusEnum.Active)).ToList();

        if (subscriptionPurchased.Where(x => x.SubscriptionStatus == UserPackageStatusEnum.TempActive).Count() > 0)
        {
            var tempPckList = _context.SubscriptionHistories.Include(x => x.Subscription)
            .Where(p => p.CreatedBy == userId &&
            p.SubscriptionStatus == UserPackageStatusEnum.TempActive).OrderBy(x => x.Created).ToList();

            if (type == GameTypeEnum.Ranked)
            {
                if (tempPckList != null && tempPckList.Count() > 0)
                {
                    foreach (var item in tempPckList)
                    {
                        if (item.RemainingFreeRankedGames == 0 && item.IsFreeRankedUnlimited == false) { }
                        else
                        {
                            if (isPurchasedPackFound == false)
                            {
                                purchasedPck = item;
                                isPurchasedPackFound = true;
                            }
                        }
                    }
                }
            }
            else if (type == GameTypeEnum.Unranked)
            {
                if (tempPckList != null && tempPckList.Count() > 0)
                {
                    foreach (var item in tempPckList)
                    {
                        if (item.RemainingFreeUnrankedGames == 0 && item.IsFreeUnrankedUnlimited == false) { }
                        else
                        {
                            if (isPurchasedPackFound == false)
                            {
                                purchasedPck = item;
                                isPurchasedPackFound = true;
                            }
                        }
                    }
                }
            }
        }

        if (subscriptionPurchased.Where(x => x.SubscriptionStatus == UserPackageStatusEnum.Active && x.Subscription.SubscriptionType != SubscriptionTypeEnum.FreemiumModel).Count() > 0
            && isPurchasedPackFound == false)
        {
            var tempPckList = subscriptionPurchased.Where(x => x.SubscriptionStatus == UserPackageStatusEnum.Active && x.Subscription.SubscriptionType != SubscriptionTypeEnum.FreemiumModel).FirstOrDefault();

            if (type == GameTypeEnum.Ranked)
            {
                if (tempPckList != null)
                {

                    if (tempPckList.RemainingFreeRankedGames == 0 && tempPckList.IsFreeRankedUnlimited == false) { }
                    else
                    {
                        if (isPurchasedPackFound == false)
                        {
                            purchasedPck = tempPckList;
                            isPurchasedPackFound = true;
                        }
                    }
                }
            }
            else if (type == GameTypeEnum.Unranked)
            {
                if (tempPckList != null)
                {
                    if (tempPckList.RemainingFreeUnrankedGames == 0 && tempPckList.IsFreeUnrankedUnlimited == false) { }
                    else
                    {
                        if (isPurchasedPackFound == false)
                        {
                            purchasedPck = tempPckList;
                            isPurchasedPackFound = true;
                        }
                    }
                }
            }
        }

        if (isPurchasedPackFound == false)
        {
            var tempPckList = subscriptionPurchased.Where(x => x.Subscription.SubscriptionType == SubscriptionTypeEnum.FreemiumModel).FirstOrDefault();

            if (type == GameTypeEnum.Ranked)
            {
                if (tempPckList != null)
                {
                    if (tempPckList.RemainingFreeRankedGames == 0 && tempPckList.IsFreeRankedUnlimited == false) { }
                    else
                    {
                        if (isPurchasedPackFound == false)
                        {
                            purchasedPck = tempPckList;
                            isPurchasedPackFound = true;
                        }
                    }
                }
            }
            else if (type == GameTypeEnum.Unranked)
            {
                if (tempPckList != null)
                {
                    if (tempPckList.RemainingFreeUnrankedGames == 0 && tempPckList.IsFreeUnrankedUnlimited == false) { }
                    else
                    {
                        if (isPurchasedPackFound == false)
                        {
                            purchasedPck = tempPckList;
                            isPurchasedPackFound = true;
                        }
                    }
                }
            }
        }

        return purchasedPck;

    }

    public async Task<SubscriptionHistory> GetRefundPackageDetailAsync(string userId)
    {
        SubscriptionHistory purchasedPck = null;
        var isPurchasedPackFound = false;

        var subscriptionPurchased = _context.SubscriptionHistories.Include(x => x.Subscription)
            .Where(p => p.CreatedBy == userId &&
            (p.SubscriptionStatus == UserPackageStatusEnum.TempActive || p.SubscriptionStatus == UserPackageStatusEnum.Active)).ToList();

        if (subscriptionPurchased.Where(x => x.SubscriptionStatus == UserPackageStatusEnum.Active && x.Subscription.SubscriptionType != SubscriptionTypeEnum.FreemiumModel).Count() > 0)
        {
            var activePck = subscriptionPurchased.Where(x => x.SubscriptionStatus == UserPackageStatusEnum.Active && x.Subscription.SubscriptionType != SubscriptionTypeEnum.FreemiumModel).FirstOrDefault();

            if (activePck != null)
            {
                purchasedPck = activePck;
                isPurchasedPackFound = true;
            }
        }

        if (subscriptionPurchased.Where(x => x.SubscriptionStatus == UserPackageStatusEnum.TempActive).Count() > 0
            && isPurchasedPackFound == false)
        {
            var tempPck = _context.SubscriptionHistories.Include(x => x.Subscription)
           .Where(p => p.CreatedBy == userId &&
           p.SubscriptionStatus == UserPackageStatusEnum.TempActive).OrderByDescending(x => x.Created).FirstOrDefault();

            if (tempPck != null)
            {
                purchasedPck = tempPck;
                isPurchasedPackFound = true;
            }
        }

        if (isPurchasedPackFound == false)
        {
            var freePck = subscriptionPurchased.Where(x => x.Subscription.SubscriptionType == SubscriptionTypeEnum.FreemiumModel).FirstOrDefault();

            if (freePck != null)
            {
                purchasedPck = freePck;
                isPurchasedPackFound = true;
            }
        }

        return purchasedPck;

    }

    public LeagueRankingUserContract GetUserDetail(string userId)
    {
        var user = _userManager.Users.Where(x => x.Id == userId).FirstOrDefault();

        return new LeagueRankingUserContract
        {
            id = userId,
            name = user.Fullname ?? "N/A",
            phone = user.PhoneNumber ?? "N/A",
            profilePic = user.ProfileImageUrl ?? "",
            email = user.Email ?? "N/A",
            points = (int)Math.Round(user.Points),
            rank = "Not decided",
        };
    }
    public Point GetLocation(string userId)
    {
        var user = _userManager.Users.FirstOrDefault(u => u.Id == userId);

        return user.Location;
    }

    public double GetUserRating(string userId)
    {
        var user = _userManager.Users.FirstOrDefault(u => u.Id == userId);

        return (double)user.Rating;
    }
    public bool VerifyUser(string userId)
    {
        var user = _userManager.Users.FirstOrDefault(u => u.Id == userId);
        if (user == null)
            return true;
        else
            return false;
    }

    public string GetUserName(string userId)
    {
        var user = _userManager.Users.FirstOrDefault(u => u.Id == userId);

        return user?.Fullname ?? "N/A";
    }
    public string GetTimezone(string userId)
    {
        var user = _userManager.Users.FirstOrDefault(u => u.Id == userId);

        return user?.TimezoneId ?? "N/A";
    }
    public int GetUserGender(string userId)
    {
        var user = _userManager.Users.FirstOrDefault(u => u.Id == userId);
        return (int)user.Gender;
    }
    public int GetUserPoints(string userId)
    {
        var user = _userManager.Users.FirstOrDefault(u => u.Id == userId);
        return (int)user.Points;
    }
    public int GetUserLevel(string userId)
    {
        var user = _userManager.Users.FirstOrDefault(u => u.Id == userId);
        return (int)user.Level;
    }
    public string GetTeamCode(string userId, int matchId)
    {
        var user = _context.MatchMembers.Where(x => x.MemberId == userId && x.TennisMatchId == matchId).FirstOrDefault();

        if (user != null)
            return user.TeamCode;
        else
            return "";
    }
    public async Task<UserProfileInfoDetailContract> GetAccountProfileAsync(GetAccountProfileQuery request)
    {
        //Get User Detail From database and check user exist 
        var user = await _userManager.FindByIdAsync(_currentUser.UserId);
        if (user == null)
            throw new NotFoundException($"User with {_currentUser.UserId} does not exist");

        if (request.userId != "" && request.userId != null)
        {
            user = await _userManager.FindByIdAsync(request.userId);
            if (user == null)
                throw new NotFoundException($"User with {request.userId} does not exist");
        }

        //Get current user league
        var userLeague = await _context.UserSettings.Include(x => x.SubLeague).Where(x => x.UserId == user.Id).FirstOrDefaultAsync();
        var leagueUsers = await _context.UserSettings.Where(x => x.SubLeagueId == userLeague.SubLeagueId).Select(x => x.UserId).ToListAsync();

        //var rank = _userManager.Users.Where(x => leagueUsers.Contains(x.Id)).ToList().OrderByDescending(x => x.Points).Select((user, i) => GetRank(i)).FirstOrDefault();

        var purchasedPackage = _context.SubscriptionHistories.Include(x => x.Subscription)
            .Where(p => p.CreatedBy == _currentUser.UserId && p.SubscriptionStatus == UserPackageStatusEnum.Active && p.Subscription.SubscriptionType == SubscriptionTypeEnum.FreemiumModel).FirstOrDefault();

        //Active packages
        var activePck = _context.SubscriptionHistories.Include(x => x.Subscription)
        .Where(p => p.CreatedBy == _currentUser.UserId && p.SubscriptionStatus == UserPackageStatusEnum.Active && p.Subscription.SubscriptionType != SubscriptionTypeEnum.FreemiumModel).FirstOrDefault();
        if (activePck != null)
            purchasedPackage = activePck;

        var tempPck = _context.SubscriptionHistories.Include(x => x.Subscription)
        .Where(p => p.CreatedBy == _currentUser.UserId && p.SubscriptionStatus == UserPackageStatusEnum.TempActive).OrderByDescending(x => x.Created).FirstOrDefault();
        if (activePck == null && tempPck != null)
            purchasedPackage = tempPck;

        //Check for benifit
        var tempActivePackages = _context.SubscriptionHistories.Include(x => x.Subscription)
            .Where(p => p.CreatedBy == _currentUser.UserId && (p.SubscriptionStatus == UserPackageStatusEnum.Active ||
            p.SubscriptionStatus == UserPackageStatusEnum.TempActive || p.Subscription.SubscriptionType == SubscriptionTypeEnum.FreemiumModel)).ToList();

        bool ratingCheck = false, reviewCheck = false, scoreCheck = false, matchBalance = false
            , freeContentAvailable = false, paidContentAvailable = false;
        foreach (var item in tempActivePackages)
        {
            if (item.IsRatingAvailable)
                ratingCheck = true;
            if (item.IsReviewsAvailable)
                reviewCheck = true;
            if (item.IsScoreAvailable)
                scoreCheck = true;
            if (item.IsMatchBalanceAvailable)
                matchBalance = true;
            if (item.IsFreeCouchingContentAvailable)
                freeContentAvailable = true;
            if (item.IsPaidCouchingContentAvailable)
                paidContentAvailable = true;
        }

        var rank = _userManager.Users.AsEnumerable().Where(x => leagueUsers.Contains(x.Id)).OrderByDescending(x => x.Points).Select((a, i) => new
        {
            id = a.Id,
            rank = GetRank(i),
        }).ToList();
        
        var myRank = rank.Where(x => x.id == _currentUser.UserId).Select(x => x.rank).FirstOrDefault();

        return new UserProfileInfoDetailContract
        {
            id = user.Id,
            about = user.About,
            dateOfBirth = user.DateOfBrith.ToString(_dateTime.longDateFormat),
            purchasedSubscription = new SubscriptionDtlsContract
            {
                id = purchasedPackage.Id,
                subscriptionId = purchasedPackage.SubscriptionId,
                title = purchasedPackage.Subscription.Title,
                subscriptionType = purchasedPackage.Subscription.SubscriptionType,
                durationType = purchasedPackage.Subscription.DurationType,
                price = Math.Round((double)purchasedPackage.Price, 3),
                priceAfterDiscount = Math.Round((double)purchasedPackage.PriceAfterDiscount, 3),
                isDiscountAvailable = purchasedPackage.IsDiscountAvailable,
                discount = Math.Round(purchasedPackage.Discount.Value, 3),

                costPerRankedGame = Math.Round((double)purchasedPackage.CostPerRankedGame, 3),
                costPerUnrankedGame = Math.Round((double)purchasedPackage.CostPerUnrankedGame, 3),

                isFreeCouchingContentAvailable = freeContentAvailable,
                isPaidCouchingContentAvailable = paidContentAvailable,

                isFreeRankedGameUnlimited = purchasedPackage.IsFreeRankedUnlimited,
                isFreeUnrankedGameUnlimited = purchasedPackage.IsFreeUnrankedUnlimited,
                freeRankedGames = purchasedPackage.FreeRankedGames,
                freeUnrankedGames = purchasedPackage.FreeUnrankedGames,

                isRatingAvailable = ratingCheck,
                isMatchBalanceAvailable = matchBalance,
                isScoreAvailable = scoreCheck,
                isReviewAvailable = reviewCheck,

                remainingFreeRankedMatches = purchasedPackage.RemainingFreeRankedGames,
                remainingFreeUnrankedMatches = purchasedPackage.RemainingFreeUnrankedGames,
            },
            isSubscriptionPurchased = user.isSubscriptionPurchased,
            address = user.Address,
            clubName = user.ClubName,
            dtbPerformanceClass = user.DtbPerformanceClass,
            latitute = user.Location.Y,
            longitute = user.Location.X,
            level = user.Level,
            monthPlayTime = user.MonthPlayTime,
            name = user.Fullname ?? "",
            phone = user.PhoneNumber,
            playingTennis = user.PlayingTennis,
            points = Math.Round(user.Points),
            profilePic = user.ProfileImageUrl ?? "",
            ratings = user.Rating,
            reviewedPersonCount = user.ReviewPersonCount,
            email = user.Email ?? "",
            isEmailConfirmed = user.EmailConfirmed,
            loginRole = user.LoginRole,
            gender = user.Gender,
            league = _context.UserSettings.Include(x => x.SubLeague).Where(x => x.UserId == user.Id).Select(x => new LeagueDetailContract
            {
                id = x.SubLeagueId,
                name = x.SubLeague.Name ?? "",
                iconUrl = x.SubLeague.Icon ?? "",
                minRange = x.SubLeague.MinRange,
                maxRange = x.SubLeague.MaxRange,
                myRanking = myRank,
                point = _context.LeagueRewards.Where(a => a.SubLeagueId == x.SubLeagueId).Select(f => new GenericIconInfoContract
                {
                    id = f.Id,
                    name = f.Detail,
                    iconUrl = f.Icon,
                }).ToList(),

            }).FirstOrDefault() ?? new LeagueDetailContract(),
            warnings = _context.ProfileWarnings.Where(x => x.IssuedUser == user.Id).Select(e => new WarningContract
            {
                id = e.Id,
                title = e.Title,
                description = e.Description,
                createdAt = ((DateTimeOffset)e.Created).local(user.TimezoneId).ToString(_dateTime.longDateFormat),
            }).ToList(),
            interests = _context.UserInterests.Where(x => x.CreatedBy == user.Id).Select(x => x.Name).ToList(),
            radius = user.Radius,
        };
    }
    public async Task<LeagueRankingContract> GetLeagueRanking(CancellationToken token)
    {
        //Get User Detail From database and check user exist 
        var user = await _userManager.FindByIdAsync(_currentUser.UserId);
        if (user == null)
            throw new NotFoundException($"User with {_currentUser.UserId} does not exist");

        //Get current user league
        var userLeague = await _context.UserSettings.Include(x => x.SubLeague).Where(x => x.UserId == user.Id).FirstOrDefaultAsync();
        var leagueUsers = await _context.UserSettings.Where(x => x.SubLeagueId == userLeague.SubLeagueId).Select(x => x.UserId).ToListAsync();

        return new LeagueRankingContract
        {
            id = userLeague.SubLeagueId,
            icon = userLeague.SubLeague.Icon ?? "",
            created = ((DateTimeOffset)userLeague.SubLeague.Created).local(user.TimezoneId).ToString(_dateTime.longDateFormat),
            name = userLeague.SubLeague.Name,
            minRange = userLeague.SubLeague.MinRange,
            maxRange = userLeague.SubLeague.MaxRange,
            leagueUsers = _userManager.Users.Where(x => leagueUsers.Contains(x.Id) && x.LoginRole == UserTypeEnum.User).ToList().OrderByDescending(x => x.Points).Select((a, i) => new LeagueRankingUserContract
            {
                id = a.Id,
                name = a.Fullname,
                email = a.Email,
                phone = a.PhoneNumber,
                points = (int)Math.Round(a.Points),
                profilePic = a.ProfileImageUrl ?? "",
                rank = GetRank(i),
            }).ToList(),
        };
    }

    public async Task<List<LeagueRankingUserContract>> LeagueRankingUserContract(int leagueId, CancellationToken token)
    {
        var user = await _userManager.FindByIdAsync(_currentUser.UserId);
        if (user == null)
            throw new NotFoundException($"User with {_currentUser.UserId} does not exist");

        return _context.UserSettings.Where(x => x.SubLeagueId == leagueId).ToList().Select(a => new LeagueRankingUserContract
        {
            id = a.UserId,
            name = _userManager.Users.Where(x => x.Id == a.UserId).Select(x => x.Fullname).FirstOrDefault() ?? "",
            email = _userManager.Users.Where(x => x.Id == a.UserId).Select(x => x.Fullname).FirstOrDefault() ?? "",
            phone = _userManager.Users.Where(x => x.Id == a.UserId).Select(x => x.Fullname).FirstOrDefault() ?? "",
            points = _userManager.Users.Where(x => x.Id == a.UserId).Select(x => (int)x.Points).FirstOrDefault(),
            profilePic = _userManager.Users.Where(x => x.Id == a.UserId).Select(x => x.ProfileImageUrl).FirstOrDefault(),
            rank = "---",
        }).ToList();
    }

    public async Task<PaginationResponseBaseWithActionRequest<List<TennisMatchContract>>> GetTennisMatches(GetTennisDetailPaginationQuery request, CancellationToken token)
    {
        var user = _userManager.Users.Where(x => x.Id == _currentUser.UserId).FirstOrDefault();
        var timezoneId = user.TimezoneId;

        //Creating pagination filters according to request
        var validFilter = new PaginationRequestBase(request?.query?.pageNumber, request?.query?.pageSize);
        //count all records before filtering
        var totalRecords = 0;

        List<TennisMatchContract> tennisMatch = new List<TennisMatchContract>();

        if (request.query.data?.filters.Where(u => u.actionType == ActionFilterEnum.MatchType).Count() < 1)
            throw new NotFoundException("Match type is required");

        var item = request.query.data.filters.Where(u => u.actionType == ActionFilterEnum.MatchType).First();
        var contentType = Convert.ToInt32(item.val);

        var userGeoLocation = user.Location;
        var userRadius = user.Radius;
        var userGender = (int)user.Gender;


        //Current User Leage
        var currentUserLeage = await _context.UserSettings.Include(x => x.SubLeague).Where(x => x.UserId == _currentUser.UserId).FirstOrDefaultAsync();

        var userPoints = user.Points;
        var lowerPointRange = _context.SubLeagues.Where(x => x.MaxRange < currentUserLeage.SubLeague.MaxRange).Take(2).OrderBy(x => x.MaxRange).Select(x => x.MaxRange).FirstOrDefault();
        var upperPointRange = _context.SubLeagues.Where(x => x.MaxRange > currentUserLeage.SubLeague.MaxRange).Take(2).OrderBy(x => x.MaxRange).Select(x => x.MaxRange).LastOrDefault();

        var query = new List<TennisMatch>();
        if (contentType == 0)
        {
            //Show other user matches with filter irrespective joined or not
            query = _context.TennisMatches.Include(x => x.MatchMembers).AsEnumerable()
                  .Where(u => (u.Location.ProjectTo(2855).Distance(userGeoLocation.ProjectTo(2855)) / 1000) <= userRadius
                  && u.IsMembersLimitFull == false && GetUserPoints(u.CreatedBy) >= lowerPointRange
                  && GetUserPoints(u.CreatedBy) <= upperPointRange
                  && u.CreatedBy != user.Id && u.Deleted == false
                  && !u.MatchMembers.Select(x => x.MemberId).ToList().Contains(user.Id)
                  && u.Status == MatchStatusEnum.Initial
                     && (u.Type == MatchTypeEnum.Mixed ||
                     (u.Type == MatchTypeEnum.Double && userGender == GetUserGender(u.CreatedBy))
                     || (u.Type == MatchTypeEnum.Single && userGender == GetUserGender(u.CreatedBy)))
                     ).ToList();

            totalRecords = query.Count();
        }
        else if (contentType == 1)
        {
            //Show own matches with filter

            query = _context.TennisMatches.Include(x => x.MatchReviews).Include(x => x.MatchMembers).AsEnumerable()
                  .Where(u => u.CreatedBy == _currentUser.UserId && u.Deleted == false
                && u.Status != MatchStatusEnum.Rated
                && u.Status != MatchStatusEnum.Cancelled
                && u.Status != MatchStatusEnum.Expired
                && ((u.Type == MatchTypeEnum.Single && u.MatchReviews.Where(x => x.CreatedBy == user.Id).Count() < 1) ||
                (u.Type != MatchTypeEnum.Single && u.MatchReviews.Where(x => x.CreatedBy == user.Id).Count() < 2))).ToList();

            totalRecords = query.Count();
        }
        else if (contentType == 2)
        {
            //Show own matches with filter

            query = _context.TennisMatches.Include(x => x.MatchMembers).Include(x => x.MatchReviews)
                .AsEnumerable().Where(u => u.CreatedBy != _currentUser.UserId
                && u.MatchMembers.Where(x => x.MemberId == user.Id).Any() &&
                u.Status != MatchStatusEnum.Rated && u.Deleted == false
                && u.Status != MatchStatusEnum.Cancelled
                && u.Status != MatchStatusEnum.Expired
                && ((u.Type == MatchTypeEnum.Single && u.MatchReviews.Where(x => x.CreatedBy == user.Id).Count() < 1) ||
                (u.Type != MatchTypeEnum.Single && u.MatchReviews.Where(x => x.CreatedBy == user.Id).Count() < 2))).ToList();

            totalRecords = query.Count();
        }

        tennisMatch = query.OrderBy(x => x.MatchDateTime)
                  .Select(e => new TennisMatchContract
                  {
                      id = e.Id,
                      isJoined = e.MatchMembers.Where(x => x.MemberId == user.Id).Any(),
                      startDate = ((DateTimeOffset)e.MatchDateTime).local(timezoneId).ToString(_dateTime.dayFormat),
                      startDateTime = ((DateTimeOffset)e.MatchDateTime).local(timezoneId).ToString(_dateTime.longDayDateFormat),
                      startDateTimeDate = e.MatchDateTime,
                      status = e.Status,
                      address = e.Address,
                      isMyMatch = e.CreatedBy == user.Id ? true : false,
                      latitute = e.Location.Y,
                      longitute = e.Location.X,
                      level = (LevelEnum)GetUserLevel(e.CreatedBy),
                      matchCategory = e.MatchCategory,
                      matchType = e.Type,
                      thumbnail = e.MatchImage,
                      title = e.Title,
                  }).Skip((validFilter.pageNumber.Value - 1) * validFilter.pageSize.Value)
                    .Take(validFilter.pageSize.Value).ToList();



        //Filter by search
        if (request.query.data?.filters.Where(u => u.actionType == ActionFilterEnum.Search).Count() > 0)
        {
            var searchValue = request.query.data?.filters.Where(u => u.actionType == ActionFilterEnum.Search).Select(u => u.val).First() ?? "";

            //Show own matches with filter
            totalRecords = contentType == 0 ? _context.TennisMatches.Include(x => x.MatchMembers).AsEnumerable()
                  .Where(u => u.Location.ProjectTo(2855).Distance(userGeoLocation.ProjectTo(2855)) / 1000 <= userRadius
                  && u.IsMembersLimitFull == false && GetUserPoints(u.CreatedBy) >= lowerPointRange
                  && GetUserPoints(u.CreatedBy) <= upperPointRange
                  && u.CreatedBy != user.Id && u.Deleted == false
                   && !u.MatchMembers.Select(x => x.MemberId).ToList().Contains(user.Id)
                  && u.Status == MatchStatusEnum.Initial
                 && (u.Type == MatchTypeEnum.Mixed ||
                     (u.Type == MatchTypeEnum.Double && userGender == GetUserGender(u.CreatedBy))
                     || (u.Type == MatchTypeEnum.Single && userGender == GetUserGender(u.CreatedBy)))
                  && u.Title.ToLower().Contains(searchValue.ToLower().Trim())).Count()

                  : contentType == 1 ? _context.TennisMatches.Include(x => x.MatchMembers).Include(x => x.MatchReviews).AsEnumerable()
                .Where(u => u.CreatedBy == _currentUser.UserId
                 && u.Status != MatchStatusEnum.Rated && u.Deleted == false
                && u.Status != MatchStatusEnum.Cancelled
                && u.Status != MatchStatusEnum.Expired
                && ((u.Type == MatchTypeEnum.Single && u.MatchReviews.Where(x => x.CreatedBy == user.Id).Count() < 1) ||
                (u.Type != MatchTypeEnum.Single && u.MatchReviews.Where(x => x.CreatedBy == user.Id).Count() < 2))
                && u.Title.ToLower().Contains(searchValue.ToLower().Trim())).Count()

                : _context.TennisMatches.Include(x => x.MatchMembers).Include(x => x.MatchReviews).AsEnumerable()
                .Where(u => u.CreatedBy != user.Id
                && u.Status != MatchStatusEnum.Rated && u.Deleted == false
                && u.Status != MatchStatusEnum.Cancelled
                && u.Status != MatchStatusEnum.Expired
                && ((u.Type == MatchTypeEnum.Single && u.MatchReviews.Where(x => x.CreatedBy == user.Id).Count() < 1) ||
                (u.Type != MatchTypeEnum.Single && u.MatchReviews.Where(x => x.CreatedBy == user.Id).Count() < 2))
                && u.MatchMembers.Where(x => x.MemberId == user.Id).Any() && u.Title.ToLower().Contains(searchValue.ToLower().Trim())).Count();

            tennisMatch = tennisMatch.Where(p => p.title.ToLower().Contains(searchValue.ToLower().Trim())).OrderBy(x => x.startDateTimeDate).ToList();
        }

        var totalPages = ((double)totalRecords / (double)validFilter.pageSize.Value);
        int roundedTotalPages = Convert.ToInt32(Math.Ceiling(totalPages));

        var respose = new PaginationResponseBaseWithActionRequest<List<TennisMatchContract>>(tennisMatch, request.query.data, validFilter.pageNumber ?? 0, validFilter.pageSize ?? 0, roundedTotalPages, totalRecords);
        return respose;
    }
    public async Task<List<MatchRequestUsersContract>> GetMatchRequestAbleUsers(GetMatchRequestAbleUserPaginationQuery request, CancellationToken token)
    {
        var user = _userManager.Users.Where(x => x.Id == _currentUser.UserId).FirstOrDefault();
        var timezoneId = user.TimezoneId;

        //Match detail
        var match = await _context.TennisMatches.Include(x => x.MatchMembers).Where(x => x.Id == request.matchId).FirstOrDefaultAsync();
        if (match == null)
            throw new NotFoundException("Match not found");

        //Owner of match
        var userGeoLocation = user.Location;
        var userRadius = user.Radius;
        var userGender = user.Gender;

        //Current User Leage
        var currentUserLeage = await _context.UserSettings.Include(x => x.SubLeague).Where(x => x.UserId == _currentUser.UserId).FirstOrDefaultAsync();

        var userPoints = user.Points;
        var lowerPointRange = _context.SubLeagues.Where(x => x.MaxRange < currentUserLeage.SubLeague.MaxRange).Take(2).OrderBy(x => x.MaxRange).Select(x => x.MaxRange).FirstOrDefault();
        var upperPointRange = _context.SubLeagues.Where(x => x.MaxRange > currentUserLeage.SubLeague.MaxRange).Take(2).OrderBy(x => x.MaxRange).Select(x => x.MaxRange).LastOrDefault();

        //Show other user matches with filter irrespective joined or not
        List<ApplicationUser> query = _userManager.Users.AsEnumerable()
              .Where(u => (match.Location).ProjectTo(2855).Distance((u.Location).ProjectTo(2855)) / 1000 <= u.Radius
              && match.IsMembersLimitFull == false && u.Points >= lowerPointRange
              && u.Points <= upperPointRange && match.Status == MatchStatusEnum.Initial
              && u.Id != match.CreatedBy && !match.MatchMembers.Select(x => x.MemberId).ToList().Contains(u.Id)).ToList();

        if (match.Type != MatchTypeEnum.Mixed)
            query = query.Where(x => x.Gender == userGender).ToList();

        var requestAbleUser = query
                  .Select(e => new MatchRequestUsersContract
                  {
                      id = e.Id,
                      email = e.Email,
                      name = e.Fullname,
                      phone = e.PhoneNumber,
                      profilePic = e.ProfileImageUrl ?? "",
                      point = e.Points,
                      isRequested = _context.MatchJoinRequests.Where(x => x.TennisMatchId == match.Id && x.MemberId == e.Id).Any(),
                      requestCreatedAt = _context.MatchJoinRequests.Where(x => x.TennisMatchId == match.Id && x.MemberId == e.Id)
                      .Select(x => ((DateTimeOffset)x.Created).local(timezoneId).ToString(_dateTime.longDateFormat)).FirstOrDefault() ?? "",
                      requestStatus = _context.MatchJoinRequests.Where(x => x.TennisMatchId == match.Id && x.MemberId == e.Id).Select(x => x.RequestStatus).FirstOrDefault(),
                  }).ToList();


        return requestAbleUser;
    }
    public List<MatchRequestUsersContract> GetMatchRequestUserDetail(int matchId, string createdBy)
    {

        var user = _userManager.Users.Where(x => x.Id == _currentUser.UserId).FirstOrDefault();
        var timezoneId = user.TimezoneId;

        return _context.MatchJoinRequests.Include(x => x.TennisMatch).Where(x => x.TennisMatchId == matchId && x.TennisMatch.CreatedBy == createdBy).Select(match => new MatchRequestUsersContract
        {
            id = _userManager.Users.Where(x => x.Id == match.MemberId).Select(x => x.Id).FirstOrDefault(),
            name = _userManager.Users.Where(x => x.Id == match.MemberId).Select(x => x.Fullname).FirstOrDefault(),
            phone = _userManager.Users.Where(x => x.Id == match.MemberId).Select(x => x.PhoneNumber).FirstOrDefault(),
            profilePic = _userManager.Users.Where(x => x.Id == match.MemberId).Select(x => x.ProfileImageUrl).FirstOrDefault(),
            email = _userManager.Users.Where(x => x.Id == match.MemberId).Select(x => x.Email).FirstOrDefault(),
            requestCreatedAt = ((DateTimeOffset)match.Created).local(timezoneId).ToString(_dateTime.longDayDateFormat),
            requestStatus = match.RequestStatus,
            isRequested = true,
        }).ToList();
    }

    //Get User Detail
    public async Task<DatatableResponse<List<UserProfileInfoDetailStatusContract>>> GetAllUsersDetailAsync(DataTablePaginationFilter validFilter, CancellationToken token)
    {
        //Return Customers Detail
        IQueryable<UserProfileInfoDetailStatusContract> query = _userManager.Users.Where(u => u.LoginRole == UserTypeEnum.User && u.EmailConfirmed == true).Select(u => new UserProfileInfoDetailStatusContract
        {
            id = u.Id,
            about = u.About,
            dateOfBirth = u.DateOfBrith.ToString(_dateTime.shortDateFormat),
            purchasedSubscription = new SubscriptionDtlsContract(),
            isSubscriptionPurchased = u.isSubscriptionPurchased,
            address = u.Address,
            clubName = u.ClubName ?? "N/A",
            dtbPerformanceClass = u.DtbPerformanceClass,
            latitute = u.Location.Y,
            longitute = u.Location.X,
            level = u.Level,
            monthPlayTime = u.MonthPlayTime,
            name = u.Fullname ?? "",
            phone = u.PhoneNumber,
            playingTennis = u.PlayingTennis,
            points = Math.Round(u.Points),
            profilePic = u.ProfileImageUrl ?? "",
            ratings = Math.Round((double)u.Rating, 2),
            reviewedPersonCount = u.ReviewPersonCount,
            email = u.Email ?? "",
            loginRole = u.LoginRole,
            gender = u.Gender,
            blockReason = u.BlockReason,
            league = _context.UserSettings.Include(x => x.SubLeague).Where(x => x.UserId == _currentUser.UserId).Select(x => new LeagueDetailContract
            {
                id = x.SubLeagueId,
                name = x.SubLeague.Name ?? "",
                iconUrl = x.SubLeague.Icon ?? "",
                minRange = x.SubLeague.MinRange,
                maxRange = x.SubLeague.MaxRange,
                myRanking = "---",
            }).FirstOrDefault() ?? new LeagueDetailContract(),
            rankedMatches = _context.MatchMembers.Include(x => x.TennisMatch).Where(x => x.MemberId == u.Id && x.TennisMatch.MatchCategory == GameTypeEnum.Ranked).Count(),
            unrankedMatches = _context.MatchMembers.Include(x => x.TennisMatch).Where(x => x.MemberId == u.Id && x.TennisMatch.MatchCategory == GameTypeEnum.Unranked).Count(),
            radius = u.Radius,
            accountStatus = u.AccountStatus,
            createdAt = ((DateTimeOffset)u.CreatedAt).local(u.TimezoneId).ToString(_dateTime.longDateFormat),
            createdAtDateTime = u.CreatedAt,
        }).AsQueryable();

        //Implement filteration
        if (validFilter.status != null && validFilter.status != 0)
        {
            //Active
            if (validFilter.status.Value == 1)
                query = query.Where(u => u.accountStatus == StatusEnum.Active).AsQueryable();
            //Blocked
            else if (validFilter.status.Value == 2)
                query = query.Where(u => u.accountStatus == StatusEnum.Blocked).AsQueryable();
        }


        //Searching
        if (!string.IsNullOrEmpty(validFilter.searchValue))
        {
            string? searchString = validFilter?.searchValue.Trim();
            query = query.Where(u =>
             u.name.Contains(searchString)
            || u.email.Contains(searchString)
            || u.phone.Contains(searchString)
            || u.clubName.Contains(searchString));
        }

        //Paginating Result
        var pagedData = await query.Select(u => new UserProfileInfoDetailStatusContract
        {
            id = u.id,
            about = u.about,
            dateOfBirth = u.dateOfBirth,
            purchasedSubscription = u.purchasedSubscription,
            isSubscriptionPurchased = u.isSubscriptionPurchased,
            address = u.address,
            clubName = u.clubName,
            dtbPerformanceClass = u.dtbPerformanceClass,
            latitute = u.latitute,
            longitute = u.longitute,
            level = u.level,
            monthPlayTime = u.monthPlayTime,
            name = u.name,
            phone = u.phone,
            playingTennis = u.playingTennis,
            points = u.points,
            profilePic = u.profilePic,
            ratings = u.ratings,
            reviewedPersonCount = u.reviewedPersonCount,
            email = u.email,
            loginRole = u.loginRole,
            gender = u.gender,
            blockReason = u.blockReason,
            league = u.league,
            radius = u.radius,
            accountStatus = u.accountStatus,
            rankedMatches = u.rankedMatches,
            unrankedMatches = u.unrankedMatches,
            createdAt = u.createdAt,
            createdAtDateTime = u.createdAtDateTime,
        }).Skip(validFilter.pageNumber ?? 0)
            .Take(validFilter.pageSize ?? 0)
        .ToListAsync(token);

        var totalRecords = await query.CountAsync(token);
        var pagedReponse = PaginationHelper.CreateDatatableReponse(pagedData, validFilter, totalRecords);
        return pagedReponse;
    }

    public async Task<int> GetUsersCounterAsync()
    {
        return await _userManager.Users.Where(u => u.LoginRole == UserTypeEnum.User).CountAsync();
    }
    //Stripe Subscription
    public async Task<string> PayByStripeSubscriptionAsync(int subscriptionId, string userId, string baseUrl, string success, string cancel, string failed, CancellationToken token)
    {
        try
        {
            //Check if user does'nt exists
            var loginUser = await _userManager.FindByIdAsync(userId);
            if (loginUser == null)
                throw new CustomInvalidOperationException(InvalidOperationErrorMessage.EntityNotFound("Login user"));

            //Payment Start
            var priceId = _context.Subscriptions.Where(u => u.Id == subscriptionId).Select(x => x.PriceId).FirstOrDefault();

            string customerId = "";
            var customerOptions = new CustomerSearchOptions
            {
                Query = $"name:'{loginUser.UserName}' AND email:'{loginUser.Email}'",
            };
            var customerService = new CustomerService();
            StripeSearchResult<Customer> customerResult = customerService.Search(customerOptions);
            if (customerResult.Data.Count > 0)
            {
                customerId = customerResult.Data[0].Id;
            }

            if (string.IsNullOrEmpty(customerId))
            {
                var createCustomerOptions = new CustomerCreateOptions
                {
                    Description = loginUser.Id,
                    Name = loginUser.UserName,
                    Email = loginUser.Email,
                };
                Customer customer = customerService.Create(createCustomerOptions);
                customerId = customer.Id;
            }

            //Payment End
            var options = new SessionCreateOptions
            {
                Customer = customerId,
                LineItems = new List<SessionLineItemOptions>
                {
                  new SessionLineItemOptions
                  {
                    Price = priceId,
                    Quantity = 1,

                  },

                },
                PaymentMethodTypes = new List<string>
                {
                    "card",
                },
                Mode = "subscription",
                SuccessUrl = baseUrl + success + "/{CHECKOUT_SESSION_ID}",
                CancelUrl = baseUrl + cancel + "?sessionId={CHECKOUT_SESSION_ID}",
            };
            var service = new SessionService();
            Session session = service.Create(options);

            return session.Url;
        }
        catch (Exception x)
        {
            string error = x.GetBaseException().Message;
            string url = baseUrl + failed + "?msg=" + error;
            return url;
        }
    }

    public async Task<string> OneTimeStripePaymentAsync(double amount, string userId, string baseUrl, string success, string cancel, string failed, CancellationToken token)
    {
        try
        {
            //Check if user does'nt exists
            var loginUser = await _userManager.FindByIdAsync(userId);
            if (loginUser == null)
                throw new CustomInvalidOperationException(InvalidOperationErrorMessage.EntityNotFound("Login user"));

            string customerId = "";
            var customerOptions = new CustomerSearchOptions
            {
                Query = $"name:'{loginUser.UserName}' AND email:'{loginUser.Email}'",
            };
            var customerService = new CustomerService();
            StripeSearchResult<Customer> customerResult = customerService.Search(customerOptions);
            if (customerResult.Data.Count > 0)
            {
                customerId = customerResult.Data[0].Id;
            }

            if (string.IsNullOrEmpty(customerId))
            {
                var createCustomerOptions = new CustomerCreateOptions
                {
                    Description = loginUser.Id,
                    Name = loginUser.UserName,
                    Email = loginUser.Email,
                };
                Customer customer = customerService.Create(createCustomerOptions);
                customerId = customer.Id;
            }

            //Payment End

            var options = new SessionCreateOptions
            {

                Customer = customerId,
                PaymentMethodTypes = new List<string>
                {
                    "card",
                },
                LineItems = new List<SessionLineItemOptions>
                {
                  new SessionLineItemOptions
                  {
                    PriceData = new SessionLineItemPriceDataOptions
                    {
                      UnitAmount = (long)(amount*100),
                      Currency = "eur",
                      ProductData = new SessionLineItemPriceDataProductDataOptions
                      {
                        Name = "Court Connect Payment",
                      },
                    },
                    Quantity = 1,

                  },
                },

                Mode = "payment",
                SuccessUrl = baseUrl + success + "/{CHECKOUT_SESSION_ID}",
                CancelUrl = baseUrl + cancel + "?sessionId={CHECKOUT_SESSION_ID}",
            };

            var service = new SessionService();
            Session session = service.Create(options);

            return session.Url;
        }
        catch (Exception x)
        {
            string error = x.GetBaseException().Message;
            string url = baseUrl + failed + "?msg=" + error;
            return url;
        }
    }

    public async Task<List<CouchingHubPurchasedHistory>> GetPurchasingUserDetailAsync(GetHubPurchasingUsersQuery request, CancellationToken token)
    {
        var user = _userManager.Users.Where(x => x.Id == _currentUser.UserId).FirstOrDefault();
        var timezoneId = user.TimezoneId;

        return await _context.PurchasedClassess.Where(x => x.CouchingHubId == request.hubId).OrderByDescending(x => x.Created).Select(u => new CouchingHubPurchasedHistory
        {
            id = "",
            date = u.Created.UtcToLocalTime(timezoneId).ToString(_dateTime.longDayDateFormat),
            email = _userManager.Users.Where(x=>x.Id == u.CreatedBy).Select(x=>x.Email).FirstOrDefault(),
            name = _userManager.Users.Where(x => x.Id == u.CreatedBy).Select(x => x.Fullname).FirstOrDefault(),
            phone = _userManager.Users.Where(x => x.Id == u.CreatedBy).Select(x => x.PhoneNumber).FirstOrDefault(),
            price = u.Price,
            profilePic = _userManager.Users.Where(x => x.Id == u.CreatedBy).Select(x => x.ProfileImageUrl).FirstOrDefault(),
        }).ToListAsync(token);
    }


    /// <summary>
    /// Class Private Methods
    /// </summary>
    /// <param name="type"></param>
    /// <returns></returns>

    public string GetRank(int index)
    {
        switch (index)
        {
            case 0:
                return "1st";
            case 1:
                return "2nd";
            case 2:
                return "3rd";
            default: return (index + 1) + "th";
        }
    }
    public double GetCategoryPoints(int type, int value)
    {
        if (type == 1)
        {
            switch (value)
            {
                case 0:
                    return 0;
                case 1:
                    return 1;
                case 2:
                    return 2;
                case 3:
                    return 3;
                case 4:
                    return 4;
                case 5:
                    return 5;
                case 6:
                    return 6;
                default: return 0;
            }
        }
        else if (type == 2)
        {
            switch (value)
            {
                case 0:
                    return 0;
                case 1:
                    return 1;
                case 2:
                    return 2;
                case 3:
                    return 3;
                case 4:
                    return 4;
                case 5:
                    return 5;
                case 6:
                    return 6;
                default: return 0;
            }
        }
        else if (type == 3)
        {
            switch (value)
            {
                case 0:
                    return 0;
                case 1:
                    return 1;
                case 2:
                    return 2;
                case 3:
                    return 3;
                default: return 0;
            }
        }
        else
        {
            switch (value)
            {
                case 0:
                    return 0;
                case 1:
                    return 1;
                case 2:
                    return 2;
                case 3:
                    return 3;
                case 4:
                    return 4;
                case 5:
                    return 5;
                case 6:
                    return 6;
                default: return 0;
            }
        }
    }


    private static Random random = new Random();
    public static string RandomString(int length)
    {
        const string chars = "ABCDEFGHIJKLMNOPQRSTUVWXYZ0123456789";
        return new string(Enumerable.Repeat(chars, length)
            .Select(s => s[random.Next(s.Length)]).ToArray());
    }

    public string GetMonthName(int month)
    {
        if (month == 1)
        {
            return "Jan";
        }
        else if (month == 2)
        {
            return "Feb";
        }
        else if (month == 3)
        {
            return "Mar";
        }
        else if (month == 4)
        {
            return "Apr";
        }
        else if (month == 5)
        {
            return "May";
        }
        else if (month == 6)
        {
            return "Jun";
        }
        else if (month == 7)
        {
            return "Jul";
        }
        else if (month == 8)
        {
            return "Aug";
        }
        else if (month == 9)
        {
            return "Sept";
        }
        else if (month == 10)
        {
            return "Oct";
        }
        else if (month == 11)
        {
            return "Nov";
        }
        else if (month == 12)
        {
            return "Dec";
        }


        else { return "No Month"; }
    }
}
