/************************************************************************
*************************************************************************
@Name :       	BackToTop - jQuery Plugin
@Revison :    	1.1
@Date : 		08/2013
@Author:     	ALPIXEL - (http://www.myjqueryplugins.com/jquery-plugin/backtotop - www.alpixel.fr)
@Support:    	FF, IE7, IE8, MAC Firefox, MAC Safari
@License :		Open Source - MIT License : http://www.opensource.org/licenses/mit-license.php

**************************************************************************
*************************************************************************/

/** BackToTop Plugin **/
(function ($) {

	$.BackToTop = {
		/**************/
		/** OPTIONS **/
		/**************/
		defaults: {
			/* vars */
			text: 'Back to top',
			autoShow: true,
			autoShowOffset: 0,
			timeEffect: 500,
			effectScroll: 'linear',
			appearMethod: 'fade',
			top: 10,
			opacity: 0.9
		},

		/*****************/
		/** Init Method **/
		/*****************/
		init: function (options) {
			/* vars **/
			opts = $.extend({}, $.BackToTop.defaults, options),
			/** Construct the link **/
			$.BackToTop._constructLink();


			/** Appear link when scrolling the window **/
			if (opts.autoShow)
				$(window).scroll(function () {
					if ($(this).scrollTop() > opts.autoShowOffset) {
						switch (opts.appearMethod) {
							case 'fade': divBack.stop(true, true).fadeIn('fast'); break;
							case 'slide': divBack.stop(true, true).slideDown('fast'); break;
							default: divBack.stop(true, true).show();
						}
					}
					else {
						switch (opts.appearMethod) {
							case 'fade': divBack.stop(true, true).fadeOut('fast'); break;
							case 'slide': divBack.stop(true, true).slideUp('fast'); break;
							default: divBack.stop(true, true).hide();
						}
					}
				});

			/** Scroll to top page on click **/
			$('#BackToTop').click(function (e) {
				e.preventDefault();
				$('body,html').animate({ scrollTop: 0 }, opts.timeEffect, opts.effectScroll);
			});
		},

		/** Create the link **/
		_constructLink: function () {
			divBack = $('<a />', {
				id: 'BackToTop',
				href: '#body',
				css: {
					bottom: opts.top,
					opacity: opts.opacity
				},
				html: '<span>' + opts.text + '</span>'
			}).prependTo('body');

			if (!opts.autoShow) divBack.show();
		}

	};

	/** Init method **/
	BackToTop = function (options) {
		$.BackToTop.init(options);
	};

})(jQuery);

