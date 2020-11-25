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
function getCookie(key) {
    let cookies = document.cookie;
    for (let c of cookies.split(';')) {
        if (!c.includes('=')) continue;
        if (c.split('=')[0] == key)
            return c.split('=')[1];
    }
    return null;
}
function setCookie(key, value) {

}
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