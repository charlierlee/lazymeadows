jQuery(function($){
	$(".megamenu").megamenu();
    $(".fancybox").fancybox({
        fitToView: true,
        autoSize: true
    });
});

$(window).load(function () {
	$('.equalize').syncHeight({ 'updateOnResize': true });
	$('.community-calendar li').syncHeight({ 'updateOnResize': true });
    $('.social-footer li > div').syncHeight({ 'updateOnResize': true });
    $('.social-footer .ym-g33 > div').syncHeight({ 'updateOnResize': true });
	$('ul.social li').syncHeight({ 'updateOnResize': true });
});
$(window).resize(function () {
    if ($(window).width() < 760) {
        $('.social-footer .ym-g33 > div').unSyncHeight();
    } else {
        $('.social-footer .ym-g33 > div').syncHeight({ 'updateOnResize': true });
    }
});
