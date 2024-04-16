using AdminPanel.Models;
using Application.Accounts.Commands;
using Application.Accounts.Queries;
using Application.Common.Interfaces;
using Application.Content.Command;
using Application.Services.Command;
using Application.Services.Commands;
using Application.Services.Commands.Match;
using Application.Services.Queries;
using Application.Subscriptions.Commands;
using Application.Subscriptions.Queries;
using Domain.Contracts;
using Domain.Enum;
using Domain.Generics;
using Domain.Helpers;
using MediatR;
using Microsoft.AspNetCore.Mvc;
using Stripe;

namespace AdminPanel.Controllers
{
    [Route("manage")]
    [ApiController]
    public class manageEntitiesController : ControllerBase
    {
        private readonly IApplicationDbContext _context;
        private readonly IHttpContextAccessor _contextVal;

        [Obsolete]
        public manageEntitiesController(IApplicationDbContext context,IHttpContextAccessor accessor)
        {
            _context = context;
            _contextVal = accessor;
        }

        public string BaseUrl()
        {
            var request = _contextVal.HttpContext.Request;

            // Now that you have the request you can select what you need from it.
            var url = request.Scheme + "://" + request.Host.Value;
            return url;
        }

        private IMediator _mediator = null!;
        protected IMediator Mediator => _mediator ??= HttpContext.RequestServices.GetRequiredService<IMediator>();


        //Data Pagination Query
        [HttpPost("getPaginationData/{route}")]
        public async Task<IActionResult> getPaginationDataAsync(int route, CancellationToken token)
        {
            try
            {
                var statusRaw = Request.Form["status"].FirstOrDefault();
                var draw = Request.Form["draw"].FirstOrDefault();
                var start = Request.Form["start"].FirstOrDefault();
                var length = Request.Form["length"].FirstOrDefault();
                var sortColumn = Request.Form["columns[" + Request.Form["order[0][column]"].FirstOrDefault() + "][name]"].FirstOrDefault();
                var sortColumnDirection = Request.Form["order[0][dir]"].FirstOrDefault();
                var searchValue = Request.Form["search[value]"].FirstOrDefault();
                int pageSize = length != null ? Convert.ToInt32(length) : 0;
                int skip = start != null ? Convert.ToInt32(start) : 0;
                int? status = statusRaw != null ? Convert.ToInt32(statusRaw) : null;

                var queryParameter = new DataTablePaginationFilter(skip, pageSize, status, sortColumn, sortColumnDirection, searchValue);
                dynamic query = null!;

                //Users
                if (route == 1)
                    query = new GetUserDetailPaginationQuery(queryParameter);
                //Match
                else if (route == 2)
                    query = new GetMatchCreatorsPaginationQuery(queryParameter);

                //Couching Hub Categories
                if (route == 3)
                    query = new CouchingHubPaginationQuery(queryParameter);

                //Subscription
                if (route == 4)
                    query = new SubscriptionPackagesPaginationQuery(queryParameter);

                //league
                if (route == 5)
                    query = new GetLeaguePaginationQuery(queryParameter);

                if (route == 6)
                    query = new GetCouchingHubCategoryPaginationQuery(queryParameter);
                if (route == 7)
                    query = new GetWarningDetailPaginationQuery(queryParameter);

                var queryExcute = await Mediator.Send(query, token);

                var jsonData = new { draw = draw, recordsFiltered = queryExcute.totalRecords, recordsTotal = queryExcute.totalRecords, data = queryExcute.data };
                return Ok(jsonData);
            }
            catch (Exception ex)
            {
                return BadRequest(ex.GetBaseException().Message);
            }
        }

        [HttpPost("getPaginationDataDetail/{userId}/{type}")]
        public async Task<IActionResult> getPaginationDataAsync(string userId, int type, CancellationToken token)
        {
            try
            {
                var statusRaw = Request.Form["status"].FirstOrDefault();
                var draw = Request.Form["draw"].FirstOrDefault();
                var start = Request.Form["start"].FirstOrDefault();
                var length = Request.Form["length"].FirstOrDefault();
                var sortColumn = Request.Form["columns[" + Request.Form["order[0][column]"].FirstOrDefault() + "][name]"].FirstOrDefault();
                var sortColumnDirection = Request.Form["order[0][dir]"].FirstOrDefault();
                var searchValue = Request.Form["search[value]"].FirstOrDefault();
                int pageSize = length != null ? Convert.ToInt32(length) : 0;
                int skip = start != null ? Convert.ToInt32(start) : 0;
                int? status = statusRaw != null ? Convert.ToInt32(statusRaw) : null;

                var queryParameter = new DataTablePaginationFilter(skip, pageSize, status, sortColumn, sortColumnDirection, searchValue);
                dynamic query = null!;

                //Match
                if (type == 1)
                    query = new GetMatchDetailByUserIdQuery(queryParameter, userId);
                //Drills
                //else if (type == 2)
                //    query = new GetDrillDetailByUserIdQuery(queryParameter, userId);

                var queryExcute = await Mediator.Send(query, token);

                var jsonData = new { draw = draw, recordsFiltered = queryExcute.totalRecords, recordsTotal = queryExcute.totalRecords, data = queryExcute.data };
                return Ok(jsonData);
            }
            catch (Exception ex)
            {
                return BadRequest(ex.GetBaseException().Message);
            }
        }

        [HttpPost("getPaginationDataByIdDetail/{subleagueId}/{type}")]
        public async Task<IActionResult> getLeaguePaginationDataAsync(int subleagueId, int type, CancellationToken token)
        {
            try
            {
                var statusRaw = Request.Form["status"].FirstOrDefault();
                var draw = Request.Form["draw"].FirstOrDefault();
                var start = Request.Form["start"].FirstOrDefault();
                var length = Request.Form["length"].FirstOrDefault();
                var sortColumn = Request.Form["columns[" + Request.Form["order[0][column]"].FirstOrDefault() + "][name]"].FirstOrDefault();
                var sortColumnDirection = Request.Form["order[0][dir]"].FirstOrDefault();
                var searchValue = Request.Form["search[value]"].FirstOrDefault();
                int pageSize = length != null ? Convert.ToInt32(length) : 0;
                int skip = start != null ? Convert.ToInt32(start) : 0;
                int? status = statusRaw != null ? Convert.ToInt32(statusRaw) : null;

                var queryParameter = new DataTablePaginationFilter(skip, pageSize, status, sortColumn, sortColumnDirection, searchValue);
                dynamic query = null!;

                //Match
                if (type == 1)
                    query = new GetSubLeaguePaginationQuery(queryParameter, subleagueId);
                //Drills
                //else if (type == 2)
                //    query = new GetDrillDetailByUserIdQuery(queryParameter, userId);

                var queryExcute = await Mediator.Send(query, token);

                var jsonData = new { draw = draw, recordsFiltered = queryExcute.totalRecords, recordsTotal = queryExcute.totalRecords, data = queryExcute.data };
                return Ok(jsonData);
            }
            catch (Exception ex)
            {
                return BadRequest(ex.GetBaseException().Message);
            }
        }

        [HttpGet("getMatchCalculationScore")]
        public async Task<IActionResult> getMatchCalculationScoreAsync([FromQuery] int matchId, CancellationToken token)
        {
            try
            {
                var res = await Mediator.Send(new GetMatchScoreHistoryQuery(matchId), token);
                return Ok(new LocalResponseModel { status = true, result = res });
            }
            catch (Exception ex)
            {
                return BadRequest(new LocalResponseModel { status = false, result = ex.GetBaseException().Message });
            }
        }

        [HttpGet("softDeleteMatch")]
        public async Task<IActionResult> softDeleteMatchAsync([FromQuery] int id, bool isDeleted, string? reason, CancellationToken token)
        {
            try
            {
                var res = await Mediator.Send(new UpdateMatchStatusCommand(id, isDeleted, reason), token);
                return Ok(new LocalResponseModel { status = true, result = res });
            }
            catch (Exception ex)
            {
                return BadRequest(new LocalResponseModel { status = false, result = ex.GetBaseException().Message });
            }
        }


        [HttpGet("softDeleteUser")]
        public async Task<IActionResult> softDeleteUserAsync([FromQuery] string id, int isDeleted, string? reason, int loginRole, CancellationToken token)
        {
            try
            {
                var res = await Mediator.Send(new StatusUpdateCommand(id, isDeleted, reason, loginRole), token);
                return Ok(new LocalResponseModel { status = true, result = res });
            }
            catch (Exception ex)
            {
                return BadRequest(new LocalResponseModel { status = false, result = ex.GetBaseException().Message });
            }
        }

        [HttpGet("softDeleteCategory")]
        public async Task<IActionResult> softDeleteCategoryAsync([FromQuery] int id, bool isDeleted, CancellationToken token)
        {
            try
            {
                var res = await Mediator.Send(new UpdateCategoryStatusCommand(id, isDeleted), token);
                return Ok(new LocalResponseModel { status = true, result = res });
            }
            catch (Exception ex)
            {
                return BadRequest(new LocalResponseModel { status = false, result = ex.GetBaseException().Message });
            }
        }

        [HttpGet("deleteWarning")]
        public async Task<IActionResult> deleteWarningAsync([FromQuery] int id, CancellationToken token)
        {
            try
            {
                var res = await Mediator.Send(new DeleteWarningCommand(id), token);
                return Ok(new LocalResponseModel { status = true, result = res });
            }
            catch (Exception ex)
            {
                return BadRequest(new LocalResponseModel { status = false, result = ex.GetBaseException().Message });
            }
        }

        [HttpPost("createcategory")]
        public async Task<IActionResult> createCategoryAsync([FromBody] CreateCategoryCommand command, CancellationToken cancellationToken)
        {
            try
            {
                var isDone = await Mediator.Send(command, cancellationToken);

                if (isDone.Succeeded)
                    return Ok(new { status = true, result = "" });
                else
                    return BadRequest(new { status = false, result = isDone.Errors.ToString() });
            }
            catch (Exception ex)
            {
                return BadRequest(new { status = false, result = ex.GetBaseException().Message });
            }
        }

        [HttpPut("updatecategory")]
        public async Task<IActionResult> updateCategoryAsync([FromBody] UpdateCategoryCommand command, CancellationToken cancellationToken)
        {
            try
            {
                var isDone = await Mediator.Send(command, cancellationToken);

                if (isDone.Succeeded)
                    return Ok(new { status = true, result = "" });
                else
                    return BadRequest(new { status = false, result = isDone.Errors.ToString() });
            }
            catch (Exception ex)
            {
                return BadRequest(new { status = false, result = ex.GetBaseException().Message });
            }
        }

        [HttpGet("getcouchingcategory")]
        public async Task<IActionResult> getCategoryAsync([FromQuery] int id, CancellationToken cancellationToken)
        {
            try
            {
                var res = await Mediator.Send(new GetCategoryDetailQuery(id), cancellationToken);
                return Ok(new LocalResponseModel { status = true, result = res });
            }
            catch (Exception ex)
            {
                return BadRequest(new LocalResponseModel { status = false, result = ex.GetBaseException().Message });
            }
        }

        /// <summary>
        ///  User Account   
        /// </summary>
        /// <param name="command"></param>
        /// <param name="cancellationToken"></param>
        /// <returns></returns>
        //[HttpGet("checkValueUniqueness/{type}/{subType?}/{key}")]
        //public async Task<IActionResult> checkValueUniquenessAsync(int type, int? subType, string key, CancellationToken token)
        //{
        //    try
        //    {
        //        var isValueUnique = await Mediator.Send(new CheckIdentityValueUniquenessQuery(type, subType, key), token);
        //        return Ok(new { status = true, result = isValueUnique });
        //    }
        //    catch (Exception ex)
        //    {
        //        return BadRequest(new { status = false, result = ex.GetBaseException().Message });
        //    }
        //}

        [HttpPost("updateProfile")]
        public async Task<IActionResult> updateProfileAsync([FromBody] UpdateProfileCommand command, CancellationToken cancellationToken)
        {
            try
            {
                var isDone = await Mediator.Send(command, cancellationToken);

                if (isDone.Succeeded)
                    return Ok(new { status = true, result = "" });
                else
                    return BadRequest(new { status = false, result = isDone.Errors.ToString() });
            }
            catch (Exception ex)
            {
                return BadRequest(new { status = false, result = ex.GetBaseException().Message });
            }
        }

        [HttpPost("updateProfilePicture")]
        public async Task<IActionResult> updateProfilePictureAsync(IFormFile file, CancellationToken cancellationToken)
        {
            try
            {
                string url = "";
                if (file == null)
                    return BadRequest(new { status = false, result = "file not found" });

                url = await UploadFile(file);

                var isDone = await Mediator.Send(new UpdateProfileCommand(null, null, url, null, null, null, null, null, null, null), cancellationToken);

                if (isDone.Succeeded)
                    return Ok(new { status = true, result = "" });
                else
                    return BadRequest(new { status = false, result = isDone.Errors.ToString() });
            }
            catch (Exception ex)
            {
                return BadRequest(new { status = false, result = ex.GetBaseException().Message });
            }
        }

        [HttpPost("changePassword")]
        public async Task<IActionResult> changePasswordAsync([FromBody] ResetPasswordCommand command, CancellationToken cancellationToken)
        {
            try
            {
                var isDone = await Mediator.Send(command, cancellationToken);

                if (isDone.Succeeded)
                    return Ok(new { status = true, result = "" });
                else
                    return BadRequest(new { status = false, result = isDone.Errors.ToString() });
            }
            catch (Exception ex)
            {
                return BadRequest(new { status = false, result = ex.GetBaseException().Message });
            }
        }

        [HttpGet("getSubscriptionByUserDetail")]
        public async Task<IActionResult> getSubscriptionDetailAsync([FromQuery] string userId, CancellationToken token)
        {
            try
            {
                var stadiumFields = await Mediator.Send(new GetSubscriptionByUserIdQuery(userId), token);

                return Ok(new LocalResponseModel { status = true, result = stadiumFields });
            }
            catch (Exception ex)
            {
                return BadRequest(new LocalResponseModel { status = false, result = ex.GetBaseException().Message });
            }
        }

        //Subscription Payment Start

        [HttpGet]
        [Route("subscription/addSubscription")]
        public ResponseDtos CreateSubscription(float? price, string? planname, int? durationtype)
        {
            try
            {

                var Price = price;

                // Stripe productId start
                var stripeSecretkey = "sk_test_51MBZSrFR0yNUey3mOBEZIVM1XjwGyB87XpAq9KCnD7S4py00V8KJQblIy2cKoyrvqzeEICNMBQ4ggPiXhKu2EubP00HqXNa6O3";
                StripeConfiguration.ApiKey = stripeSecretkey;

                var options = new ProductCreateOptions
                {
                    Name = planname,
                };
                var service = new ProductService();
                var prd = service.Create(options);

                // Stripe productId end

                // Stripe priceId start
                var type = "";
                if (durationtype == 0)
                {
                    type = "day";
                }
                else if (durationtype == 1)
                {
                    type = "week";
                }
                else if (durationtype == 2)
                {
                    type = "month";
                }
                else if(durationtype == 3)
                {
                    type = "year";
                }

                var options1 = new PriceCreateOptions
                {
                    UnitAmount = (long)Price * 100,
                    Currency = "usd",
                    Recurring = new PriceRecurringOptions
                    {
                        Interval = type,
                    },
                    Product = prd.Id,
                    TaxBehavior = "exclusive",
                };
                var service1 = new PriceService();
                var prc = service1.Create(options1);

                // Stripe priceId end
                var productId = prd.Id;
                var priceId = prc.Id;

                ResponseDtos response = new()
                {
                    priceId = priceId,
                    productId = productId
                };
                return response;
            }
            catch (Exception ex)
            {
                ResponseDtos response = new()
                {
                    priceId = null,
                    productId = null
                };
                return response;
            }
        }

        [HttpGet]
        [Route("subscription/updateSubscription")]
        public ResponseDtos EditSubscription(int packageId, string planname, float? newprice)
        {
            try
            {
                var oldprice = _context.Subscriptions.Where(x => x.Id == packageId).Select(p => p.Price).FirstOrDefault();
                var price = oldprice;
                var priceid = _context.Subscriptions.Where(x => x.Id == packageId).Select(p => p.PriceId).FirstOrDefault();
                var productId = _context.Subscriptions.Where(x => x.Id == packageId).Select(p => p.ProductId).FirstOrDefault();
                var durationtype = _context.Subscriptions.Where(x => x.Id == packageId).Select(p => p.DurationType).FirstOrDefault();
                if (price != newprice)
                {
                    var stripeSecretkey = "sk_test_51MBZSrFR0yNUey3mOBEZIVM1XjwGyB87XpAq9KCnD7S4py00V8KJQblIy2cKoyrvqzeEICNMBQ4ggPiXhKu2EubP00HqXNa6O3";
                    StripeConfiguration.ApiKey = stripeSecretkey;

                    var options = new PriceUpdateOptions
                    {
                        Active = false,
                    };
                    var service = new PriceService();
                    service.Update(priceid, options);

                    //Create New Price Start
                    var type = "";
                    if (durationtype == Domain.Enum.DurationTypeEnum.Day)
                    {
                        type = "day";
                    }
                    else if (durationtype == Domain.Enum.DurationTypeEnum.Week)
                    {
                        type = "week";
                    }
                    else if (durationtype == Domain.Enum.DurationTypeEnum.Month)
                    {
                        type = "month";
                    }
                    else if (durationtype == Domain.Enum.DurationTypeEnum.Year)
                    {
                        type = "year";
                    }

                    var amount = newprice * 100;
                    var options1 = new PriceCreateOptions
                    {
                        UnitAmount = (long)amount,
                        Currency = "usd",
                        Recurring = new PriceRecurringOptions
                        {
                            Interval = type,
                        },
                        TaxBehavior = "exclusive",
                        Product = productId,
                    };
                    var service1 = new PriceService();
                    var prc = service1.Create(options1);
                    //Create New Price End

                    priceid = prc.Id;

                }
                ResponseDtos response = new()
                {
                    priceId = priceid,
                    productId = productId
                };
                return response;
            }
            catch (Exception ex)
            {
                ResponseDtos response = new()
                {
                    priceId = null,
                    productId = null
                };
                return response;
            }
        }

        [HttpPost("createSubscription")]
        public async Task<IActionResult> createSubscriptionAsync([FromBody] AddSubscriptionCommand content, CancellationToken token)
        {
            try
            {
                var res = await Mediator.Send(content, token);
                return Ok(new LocalResponseModel { status = true, result = res });
            }
            catch (Exception ex)
            {
                return BadRequest(new LocalResponseModel { status = false, result = ex.GetBaseException().Message });
            }

        }

        [HttpPost("editSubscription")]
        public async Task<IActionResult> updateSubscriptionAsync([FromBody] UpdateSubscriptionCommand content, CancellationToken token)
        {
            try
            {
                var res = await Mediator.Send(content, token);
                return Ok(new LocalResponseModel { status = true, result = res });
            }
            catch (Exception ex)
            {
                return BadRequest(new LocalResponseModel { status = false, result = ex.GetBaseException().Message });
            }

        }


        [HttpGet("getReviewByUserDetail")]
        public async Task<IActionResult> getReviewByUserDetailAsync([FromQuery] string userId, CancellationToken token)
        {
            try
            {
                var stadiumFields = await Mediator.Send(new GetReveiwByUserQuery(userId), token);

                return Ok(new LocalResponseModel { status = true, result = stadiumFields });
            }
            catch (Exception ex)
            {
                return BadRequest(new LocalResponseModel { status = false, result = ex.GetBaseException().Message });
            }
        }

        /// <summary>
        /// Matches    
        /// </summary>
        /// <param name="userId"></param>
        /// <param name="token"></param>
        /// <returns></returns>

        //[HttpGet("getMatchByUserDetail")]
        //public async Task<IActionResult> getMatchUserDetailAsync([FromQuery] int userId, CancellationToken token)
        //{
        //    try
        //    {
        //        var stadiumFields = await Mediator.Send(new GetCourseTopicsDetailQuery(userId), token);

        //        return Ok(new LocalResponseModel { status = true, result = stadiumFields });
        //    }
        //    catch (Exception ex)
        //    {
        //        return BadRequest(new LocalResponseModel { status = false, result = ex.GetBaseException().Message });
        //    }
        //}


        /// <summary>
        /// Leagues
        /// </summary>
        /// <param name="leagueId"></param>
        /// <param name="token"></param>
        /// <returns></returns>

        [HttpGet("getSubLeagueDetail")]
        public async Task<IActionResult> getSubLeagueDetailAsync([FromQuery] int leagueId, CancellationToken token)
        {
            try
            {
                var stadiumFields = await Mediator.Send(new GetSubLeagueByLeagueIdQuery(leagueId), token);

                return Ok(new LocalResponseModel { status = true, result = stadiumFields });
            }
            catch (Exception ex)
            {
                return BadRequest(new LocalResponseModel { status = false, result = ex.GetBaseException().Message });
            }
        }

        //[HttpGet("getLeagueRankingDetail")]
        //public async Task<IActionResult> getLeagueRankingAsync([FromQuery] int leagueId, CancellationToken token)
        //{
        //    try
        //    {
        //        var stadiumFields = await Mediator.Send(new GetReviewByUserIdQuery(leagueId), token);

        //        return Ok(new LocalResponseModel { status = true, result = stadiumFields });
        //    }
        //    catch (Exception ex)
        //    {
        //        return BadRequest(new LocalResponseModel { status = false, result = ex.GetBaseException().Message });
        //    }
        //}

        /// <summary>
        /// Couching Hub
        /// </summary>
        /// <param name="userId"></param>
        /// <param name="token"></param>
        /// <returns></returns>

        //[HttpGet("getContentPurchasedUserDetail")]
        //public async Task<IActionResult> getContentPurchasedUserDetailAsync([FromQuery] int couchingHubId, CancellationToken token)
        //{
        //    try
        //    {
        //        var stadiumFields = await Mediator.Send(new GetReviewByUserIdQuery(couchingHubId, token));

        //        return Ok(new LocalResponseModel { status = true, result = stadiumFields });
        //    }
        //    catch (Exception ex)
        //    {
        //        return BadRequest(new LocalResponseModel { status = false, result = ex.GetBaseException().Message });
        //    }
        //}

        //[HttpGet("getCouchingHubContentBenifits")]
        //public async Task<IActionResult> getCouchingHubContentBenifitsAsync([FromQuery] int couchingHubId, CancellationToken token)
        //{
        //    try
        //    {
        //        var stadiumFields = await Mediator.Send(new GetReviewByUserIdQuery(couchingHubId, token));

        //        return Ok(new LocalResponseModel { status = true, result = stadiumFields });
        //    }
        //    catch (Exception ex)
        //    {
        //        return BadRequest(new LocalResponseModel { status = false, result = ex.GetBaseException().Message });
        //    }
        //}

        //[HttpGet("getCouchingHubCategoryId")]
        //public async Task<IActionResult> getCouchingHubCategoryIdAsync([FromQuery] int categoryId, CancellationToken token)
        //{
        //    try
        //    {
        //        var stadiumFields = await Mediator.Send(new GetReviewByUserIdQuery(categoryId, token));

        //        return Ok(new LocalResponseModel { status = true, result = stadiumFields });
        //    }
        //    catch (Exception ex)
        //    {
        //        return BadRequest(new LocalResponseModel { status = false, result = ex.GetBaseException().Message });
        //    }
        //}

        /// <summary>
        /// App Content
        /// </summary>
        /// <param name="content"></param>
        /// <param name="token"></param>
        /// <returns></returns>
        [HttpPost("updateAppContent")]
        public async Task<IActionResult> updateAppContentAsync([FromBody] UpdateTermContentCommand content, CancellationToken token)
        {
            try
            {
                var res = await Mediator.Send(content, token);
                return Ok(new LocalResponseModel { status = true, result = res });
            }
            catch (Exception ex)
            {
                return BadRequest(new LocalResponseModel { status = false, result = ex.GetBaseException().Message });
            }

        }

        [HttpPost("editAppContent")]
        public async Task<IActionResult> editAppContentAsync([FromBody] UpdateAboutAppContentCommand content, CancellationToken token)
        {
            try
            {
                var res = await Mediator.Send(content, token);
                return Ok(new LocalResponseModel { status = true, result = res });
            }
            catch (Exception ex)
            {
                return BadRequest(new LocalResponseModel { status = false, result = ex.GetBaseException().Message });
            }

        }

        [HttpPost("addAppContent")]
        public async Task<IActionResult> addAppContentAsync([FromBody] CreateAppContentCommand content, CancellationToken token)
        {
            try
            {
                var res = await Mediator.Send(content, token);
                return Ok(new LocalResponseModel { status = true, result = res });
            }
            catch (Exception ex)
            {
                return BadRequest(new LocalResponseModel { status = false, result = ex.GetBaseException().Message });
            }

        }

        [HttpGet("deleteAppContent")]
        public async Task<IActionResult> deleteAppContentAsync([FromQuery] int id, CancellationToken token)
        {
            try
            {
                var res = await Mediator.Send(new DeleteAppContentCommand(id), token);
                return Ok(new LocalResponseModel { status = true, result = res });
            }
            catch (Exception ex)
            {
                return BadRequest(new LocalResponseModel { status = false, result = ex.GetBaseException().Message });
            }

        }


        /// <summary>
        /// Subscription
        /// </summary>
        /// <param name="file"></param>
        /// <returns></returns>

        [HttpGet("softDeleteSubscription")]
        public async Task<IActionResult> softDeleteSubscriptionAsync([FromQuery] int id, bool isDeleted, CancellationToken token)
        {
            try
            {
                var res = await Mediator.Send(new SubscriptionStatusUpdateCommand(id, isDeleted), token);
                return Ok(new LocalResponseModel { status = true, result = res });
            }
            catch (Exception ex)
            {
                return BadRequest(new LocalResponseModel { status = false, result = ex.GetBaseException().Message });
            }
        }

        /// <summary>
        /// Couching hub
        /// </summary>
        /// <returns></returns>

        [HttpGet("getCategory")]
        public async Task<IActionResult> getCategoryAsync(CancellationToken token)
        {
            try
            {
                var stadiumFields = await Mediator.Send(new GetCouchingHubCategoryQuery(), token);

                return Ok(new LocalResponseModel { status = true, result = stadiumFields });
            }
            catch (Exception ex)
            {
                return BadRequest(new LocalResponseModel { status = false, result = ex.GetBaseException().Message });
            }
        }



        [HttpGet("getLeagueById")]
        public async Task<IActionResult> getLeagueAsync([FromQuery] int leagueId,CancellationToken token)
        {
            try
            {
                var league = await Mediator.Send(new GetLeagueQuery(leagueId), token);

                return Ok(new LocalResponseModel { status = true, result = league });
            }
            catch (Exception ex)
            {
                return BadRequest(new LocalResponseModel { status = false, result = ex.GetBaseException().Message });
            }
        }

        [HttpPut("updateLeague")]
        public async Task<IActionResult> updateLeagueAsync([FromBody] UpdateLeagueCommand content, CancellationToken token)
        {
            try
            {
                var res = await Mediator.Send(content, token);
                return Ok(new LocalResponseModel { status = true, result = res });
            }
            catch (Exception ex)
            {
                return BadRequest(new LocalResponseModel { status = false, result = ex.GetBaseException().Message });
            }

        }

        [HttpGet("getSubLeague")]
        public async Task<IActionResult> getSubLeagueAsync([FromQuery] int subleagueId, CancellationToken token)
        {
            try
            {
                var league = await Mediator.Send(new GetSubLeagueQuery(subleagueId), token);

                return Ok(new LocalResponseModel { status = true, result = league });
            }
            catch (Exception ex)
            {
                return BadRequest(new LocalResponseModel { status = false, result = ex.GetBaseException().Message });
            }
        }

        [HttpGet("getHubBenifits")]
        public async Task<IActionResult> getHubBenifitsAsync([FromQuery] int hubId, CancellationToken token)
        {
            try
            {
                var stadiumFields = await Mediator.Send(new GetHubBenifitsQuery(hubId), token);

                return Ok(new LocalResponseModel { status = true, result = stadiumFields });
            }
            catch (Exception ex)
            {
                return BadRequest(new LocalResponseModel { status = false, result = ex.GetBaseException().Message });
            }
        }

        [HttpGet("getPurchasedUser")]
        public async Task<IActionResult> getPurchasedUserAsync([FromQuery] int hubId, CancellationToken token)
        {
            try
            {
                var stadiumFields = await Mediator.Send(new GetHubPurchasingUsersQuery(hubId), token);

                return Ok(new LocalResponseModel { status = true, result = stadiumFields });
            }
            catch (Exception ex)
            {
                return BadRequest(new LocalResponseModel { status = false, result = ex.GetBaseException().Message });
            }
        }

        [HttpPost("addCouchingHub")]
        public async Task<IActionResult> addCouchingHubAsync([FromBody] AddCouchingHubContentCommand content, CancellationToken token)
        {
            try
            {
                var res = await Mediator.Send(content, token);
                return Ok(new LocalResponseModel { status = true, result = res });
            }
            catch (Exception ex)
            {
                return BadRequest(new LocalResponseModel { status = false, result = ex.GetBaseException().Message });
            }

        }

        [HttpPost("addLeague")]
        public async Task<IActionResult> addLeagueAsync([FromBody] AddLeagueCommand content, CancellationToken token)
        {
            try
            {
                var res = await Mediator.Send(content, token);
                return Ok(new LocalResponseModel { status = true, result = res });
            }
            catch (Exception ex)
            {
                return BadRequest(new LocalResponseModel { status = false, result = ex.GetBaseException().Message });
            }

        }


        [HttpPost("editCouchingHub")]
        public async Task<IActionResult> editCouchingHubAsync([FromBody] UpdateCouchingHubContent content, CancellationToken token)
        {
            try
            {
                var res = await Mediator.Send(content, token);
                return Ok(new LocalResponseModel { status = true, result = res });
            }
            catch (Exception ex)
            {
                return BadRequest(new LocalResponseModel { status = false, result = ex.GetBaseException().Message });
            }

        }

        [HttpPost("editMatchDetail")]
        public async Task<IActionResult> editMatchDetailAsync([FromBody] EditMatchCommand content, CancellationToken token)
        {
            try
            {
                var res = await Mediator.Send(content, token);
                return Ok(new LocalResponseModel { status = true, result = res });
            }
            catch (Exception ex)
            {
                return BadRequest(new LocalResponseModel { status = false, result = ex.GetBaseException().Message });
            }

        }


        [HttpGet("softDeleteCouchingHub")]
        public async Task<IActionResult> softDeleteCouchingHubAsync([FromQuery] int id, bool isDeleted, CancellationToken token)
        {
            try
            {
                var res = await Mediator.Send(new CouchingHubStatusUpdateCommand(id, isDeleted), token);
                return Ok(new LocalResponseModel { status = true, result = res });
            }
            catch (Exception ex)
            {
                return BadRequest(new LocalResponseModel { status = false, result = ex.GetBaseException().Message });
            }
        }

        //File upload functions
        [HttpPost("uploadFile")]
        public async Task<JsonResult> fileUpload(IFormFile file)
        {
            string url = "";
            if (file != null)
            {
                url = await UploadFile(file);
            }
            return new JsonResult(url);
        }

        [Obsolete]
        internal async Task<string> UploadFile(IFormFile file)
        {
            long fileSize = file.Length;
            string extension = Path.GetExtension(file.FileName);
            string ext = file.ContentType;
            string path_1 = Path.Combine(@"wwwroot", "Images");
            string uploadsFolder = Path.Join(Directory.GetCurrentDirectory(), path_1);
            if (!Directory.Exists(uploadsFolder)) Directory.CreateDirectory(uploadsFolder);
            string uniqueFileName = Guid.NewGuid().ToString() + extension;
            string filePath = Path.Join(uploadsFolder, uniqueFileName);
            using (var _fileStream = new FileStream(filePath, FileMode.Create))
                await file.CopyToAsync(_fileStream);

            return BaseUrl() + "/Images/" + uniqueFileName;
        }

        [HttpPost("base64FileUpload"), DisableRequestSizeLimit]
        public JsonResult UploadBase64File([FromBody] string file)
        {
            var folderName = Path.Combine(@"wwwroot\Images");
            var pathToSave = Path.Combine(Directory.GetCurrentDirectory(), folderName);

            //Check if directory exist
            if (!Directory.Exists(pathToSave))
                Directory.CreateDirectory(pathToSave); //Create directory if it doesn't exist

            string imageName = "Base" + DateTime.Now.Ticks.ToString() +  file.GetBase64FileType();

            //set the image path
            string imgPath = Path.Combine(pathToSave, imageName);

            byte[] imageBytes = Convert.FromBase64String(file);

            System.IO.File.WriteAllBytes(imgPath, imageBytes);
            //Get Base Url From Request
            var baseUrl = $"{this.Request.Scheme}://{this.Request.Host}{this.Request.PathBase}";
            //Final Return Url
            var dbPath = $"{baseUrl}/Images/{imageName}";

            return new JsonResult(dbPath);
        }

    }
}
