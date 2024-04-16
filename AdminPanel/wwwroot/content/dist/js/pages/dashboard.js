const loaderDiv = document.getElementById('showLoaderApi');
$.fn.serializeObject = function () {
    var o = {};
    var a = this.serializeArray();
    $.each(a, function () {
        if (o[this.name]) {
            if (!o[this.name].push) {
                o[this.name] = [o[this.name]];
            }
            o[this.name].push(this.value || '');
        } else {
            o[this.name] = this.value || '';
        }
    });
    return o;
};
function loadHttpRequest(method, url, cFunction, params ) {
    loaderDiv.innerHTML  = '<div class="loading">Loading&#8230;</div>';
    var xhttp;
    xhttp = new XMLHttpRequest();

    xhttp.addEventListener("load", transferComplete);
    xhttp.addEventListener("error", transferFailed);
    xhttp.addEventListener("abort", transferCanceled);

    xhttp.onreadystatechange = function () {        
        if (this.readyState == 4 && this.status == 200) {
            const objModel = JSON.parse(this.responseText);
            if (objModel.status) {
                cFunction(objModel.result)
            }
            else {
                toastr.error(objModel.result + " with url " + this.responseURL)                
            }
        }
    };
    xhttp.open(method, url, true);
    xhttp.setRequestHeader("Content-type", "application/json");
    xhttp.send(params);
}

function transferComplete(evt) {
    loaderDiv.innerHTML = '';
}

function transferFailed(evt) {
    console.error(evt);
}

function transferCanceled(evt) {
    console.error(evt);
}
