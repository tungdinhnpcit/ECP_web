/* Add here all your JS customizations */
function loading(name, overlay) {
    
    $('#ndContent').append('<div id="overlay"></div><div id="preloader" style="height: 40px;">' + name + '..</div>');

    if (overlay == 1) {
        $('#overlay').css('opacity', 0.4).fadeIn(400, function () { $('#preloader').fadeIn(400); });
        return false;
    }
    $('#preloader').fadeIn();
}
function unloading() {
    $('#preloader').fadeOut(400, function () { $('#overlay').fadeOut(); $(this).remove(); })
}

function showError(str) {
    $('#alertMessage').addClass('error').html(str).stop(true, true).show().animate({ opacity: 1, right: '10' }, 500);

}

function showSuccess(str) {
    $('#alertMessage').removeClass('error').addClass("success").html(str).stop(true, true).show().animate({ opacity: 1, right: '10' }, 500);
}
function hideTop() {
    $('#alertMessage').animate({ opacity: 0, right: '-20' }, 500, function () { $(this).hide(); });
}