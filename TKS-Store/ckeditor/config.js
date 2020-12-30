/**
 * @license Copyright (c) 2003-2013, CKSource - Frederico Knabben. All rights reserved.
 * For licensing, see LICENSE.html or http://ckeditor.com/license
 */

CKEDITOR.config.allowedContent = true;
CKEDITOR.editorConfig = function (config) {
	// Define changes to default configuration here. For example:
	// config.language = 'fr';
	// config.uiColor = '#AADC6E';
	config.toolbar = 'CMS';
	config.filebrowserBrowseUrl = '/fileman/index.html';
	config.filebrowserImageBrowseUrl = '/fileman/index.html?type=image';
	config.removeDialogTabs = 'link:upload;image:upload';

	config.toolbar_CMS =
	[
		{ name: 'document', items: ['Source', '-', 'Preview'] },
		{ name: 'clipboard', items: ['Cut', 'Copy', 'Paste', 'PasteText', 'PasteFromWord', '-', 'Undo', 'Redo'] },
		{ name: 'editing', items: ['Find', 'Replace', '-', 'SelectAll', '-', 'SpellChecker', 'Scayt'] },
		{ name: 'color', items: ['TextColor', 'BGColor'] },
		'/',
		{ name: 'basicstyles', items: ['Bold', 'Italic', 'Underline', 'Strike', "-", 'Subscript', 'Superscript', '-', 'RemoveFormat'] },
		{ name: 'paragraph', items: ['NumberedList', 'BulletedList', '-', 'Outdent', 'Indent', '-', 'Blockquote', '-', 'JustifyLeft', 'JustifyCenter', 'JustifyRight', 'JustifyBlock', '-', 'ShowBlocks'] },
		{ name: 'links', items: ['Link', 'Unlink', 'Anchor'] },
		'/',
		{ name: 'insert', items: ['Image', 'Flash', 'Table', 'HorizontalRule', 'SpecialChar', 'PageBreak', 'Iframe'] },
		{ name: 'styles', items: ['Styles', 'Format', 'FontSize', 'Font'] }
	];
	config.stylesSet = 'my_styles';
	config.format_tags = 'p;h1;h2;h3;h4;h5;h6;div'
};
CKEDITOR.stylesSet.add('my_styles',
[
	// Block-level styles
	{ name: 'Blue Title', element: 'h2', styles: { 'color': 'Blue' } },
	{ name: 'Red Title', element: 'h3', styles: { 'color': 'Red' } },
	{ name: 'Green Title', element: 'h4', styles: { 'color': '#0f0' } },

	// Object styles
	{ name: 'Blue Link', element: 'a', styles: { 'color': 'Blue' } },
	{ name: 'Left Image', element: 'img', styles: { 'float': 'left' } },

	// Inline styles
	{ name: 'CSS Style', element: 'span', attributes: { 'class': 'my_style' } },
	{ name: 'Marker: Yellow', element: 'span', styles: { 'background-color': 'Yellow' } }
]);