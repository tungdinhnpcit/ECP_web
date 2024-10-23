/*
Name: 			Tables / Advanced - Examples
Written by: 	Okler Themes - (http://www.vnit-tech.com)
Theme Version: 	1.4.1
*/

(function( $ ) {

	'use strict';

	var datatableInit = function() {

	    $('#datatable-default').dataTable({
	        "paging": false,
	        "ordering": true,
	        "info": false,
	        "searching": false,
	        "scrollX": false
	    });
	};

	$(function() {
		datatableInit();
	});

}).apply( this, [ jQuery ]);