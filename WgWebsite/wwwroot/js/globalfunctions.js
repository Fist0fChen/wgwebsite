function getparamval(key) {
    let loc = window.location.href;
    if (!loc.includes("?")) return "";
    let params = loc.substring(loc.indexOf("?") + 1).split("&");
    for (let param of params) {
        if (!param.includes("=")) continue;
        let kv = param.split("=");
        if (kv[0] == key) return kv[1];
    }
    return "";
}
$(document).ready(() => {
    $('.bootstrap-toggle').bootstrapToggle();
});
function highlightButtonSuccess(id, text, access) {
    if (access == undefined) access = '#';
    $(access + id).removeClass('btn-primary');
    $(access + id).addClass('btn-success');
    let temptext = $(access + id).html();
    $(access + id).html(text);
    setTimeout(e => {
        $(access + id).removeClass('btn-success');
        $(access + id).addClass('btn-primary');
        $(access + id).html(temptext);
    }, 1000)
}
function highlightButtonFail(id, text, access) {
    if (access == undefined) access = '#';
    $(access + id).removeClass('btn-primary');
    $(access + id).addClass('btn-danger');
    let temptext = $(access + id).html();
    $(access + id).html(text);
    setTimeout(e => {
        $(access + id).removeClass('btn-danger');
        $(access + id).addClass('btn-primary');
        $(access + id).html(temptext);
    }, 1000)
}
function reload() {
    window.location.href.reload();
}
/*function getCookie(key) {
    let cookies = document.cookie;
    for (let c of cookies.split(';')) {
        if (!c.includes('=')) continue;
        if (c.split('=')[0] == key)
            return c.split('=')[1];
    }
    return null;
}
function setCookie(key, value) {

}*/
let CatData = {};
function showCategories(selid, inpid, categories) {
    CatData.categories = categories;
    CatData.selid = selid;
    CatData.inpid = inpid;
    let firstcats = [];
    for (let k in CatData.categories) firstcats.push(k);
    createCategoryButtons(firstcats, ' ');
    $('#' + inpid).keyup(e => updateCategories());
}
function updateCategories() {
    let topcats = [];
    for (let k in CatData.categories) topcats.push(k);
    let text = $('#' + CatData.inpid).val();
    if (text.length < 2 || (text[text.length - 1] == ' ' && text[text.length - 2] != '.')) {
        createCategoryButtons(topcats, ' ');
        return;
    }
    let catsub = text.split(' ');
    let cs = catsub[catsub.length - 1];
    if (cs == "" && catsub.length > 1) cs = catsub[catsub.length - 2];
    if (cs.includes('.')) {
        if (CatData.categories[cs.split('.')[0]] == undefined) createCategoryButtons([], ' ');
        else createCategoryButtons(CatData.categories[cs.split('.')[0]], '.');
    }
    else {
        createCategoryButtons(topcats, ' ');
    }
}
function createCategoryButtons(cats, del) {
    if ($(document).width() < 900) {
        $('#' + CatData.selid).removeClass('btn-group').addClass('btn-group-vertical');
    }
    else {
        $('#' + CatData.selid).removeClass('btn-group-vertical').addClass('btn-group');
    }
    $('#' + CatData.selid).html('');
    for (k of cats)
        $('#' + CatData.selid).append('<button type="button" onclick="writeCategory(\'' + k + '\', \'' + del + '\')" class="btn btn-secondary">' + k + '</button>')
}
function writeCategory(cat, del) {
    let val = $('#' + CatData.inpid).val();
    if (val.length < 1) {
        $('#' + CatData.inpid).val(cat);
        return;
    }
    if (del == " " && val[val.length - 1] != del) val += del;
    else {
        if (del == ".") val = val.substring(0, val.lastIndexOf(".") + 1);
    }
    val += cat;
    if (del == " ") val += ".";
    else val += " ";
    $('#' + CatData.inpid).val(val);
    updateCategories();
}

function assignSearchKarma() {
    $('#karma-search-input').keyup(searchKarma);
}
function searchKarma(e) {
    let results = $('.searchresult');
    let key = e.target.value.toLowerCase();
    let foundone = false;
    for (t of results) {
        if ($(t).attr('name').toLowerCase().includes(key)) {
            foundone = true;
            $(t).removeClass('collapse');
        }
        else {
            $(t).addClass('collapse');
        }
    }
    if (foundone && key != "") $('#searchresults').removeClass('collapse');
    else $('#searchresults').addClass('collapse');
}
function toggleKarmaEdit() {
    $('#edit-toggle').toggleClass('active');
    if ($('#edit-toggle').hasClass('active')) {
        $('.karma-highlight-button').css('display', '');
        $('.karma-edit-button').css('display', '');
    }
    else {
        $('.karma-highlight-button').css('display', 'none');
        $('.karma-edit-button').css('display', 'none');
    }
}
function radiotoggle(id, cls, prop) {
    $('.' + cls).removeClass(prop);
    $('#' + id).addClass(prop);
}
function formatInputPrice() {
    $('.price-input').keyup(e => {
        let val = "" + $(e.target).val();
        if (val.length == 0) {
            $(e.target).val("0,00");
        }
        else if (val.length == 1) {
            $(e.target).val("0,0" + val);
        }
        else if (val.length == 2) {
            $(e.target).val("0," + val);
        }
        else {
            $(e.target).val(val.substring(0, val.length - 2) + "," + val.substring(val.length - 2));
        }
    });
}
function loginHelper() {
    let user = $('#username').val();
    let password = $('#password').val();
    document.location.href = "/login?paramUsername=" + user + "&paramPassword=" + password;
}
$(document).ready(() => {
    resizestuff();
    setTimeout(() => resizestuff(), 500);
    $(window).resize(() => {
        resizestuff();
    });
});

function resizestuff() {
    if ($(document).width() > 767) {
        $('#nav-menu').css('display', '');
        $('#scroll-container').css('height', $(window).height() + "px");
    }
    else {
        $('#scroll-container').css('height', ($(window).height() - 56) + "px");
    }
}

async function AJAXSubmit(oFormElement) {
    const formData = new FormData(oFormElement);
    try {
        const response = await fetch(oFormElement.action, {
            method: 'POST',
            headers: {
                'RequestVerificationToken': getCookie('RequestVerificationToken')
            },
            body: formData
        });
        oFormElement.elements.namedItem("result").value =
            'Result: ' + response.status + ' ' + response.statusText;
    } catch (error) {
        console.error('Error:', error);
    }
}
function getCookie(name) {
    var value = "; " + document.cookie;
    var parts = value.split("; " + name + "=");
    if (parts.length == 2) return parts.pop().split(";").shift();
}