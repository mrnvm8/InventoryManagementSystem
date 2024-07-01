﻿//This function for the tables
var $table = $('#fresh-table');

$(function () {
	$table.bootstrapTable({
		classes: 'table table-hover ',
		toolbar: '.toolbar',

		showColumns: true,
		search: true,
		buttonsClass: 'primary',
		pagination: true,
		sortable: true,
		pageSize: 12,
		pageList: [12, 24, 48, 96, 196, 384, 768],

		formatShowingRows: function (pageFrom, pageTo, totalRows) {
			return '';
		},
		formatRecordsPerPage: function (pageNumber) {
			return pageNumber + ' rows visible';
		},
	});
});

//This function for the tables
var $_table = $('#index-table');

$(function () {
	$_table.bootstrapTable({
		classes: 'table table-hover ',
		toolbar: '.toolbar',
		search: true,
	});
});

function VisiblePassword() {
	var x = document.getElementById('InputPassword');
	if (x.type == 'password') {
		x.type = 'text';
	} else {
		x.type = 'password';
	}
}

//Local global settings
$(document).ready(function () {
	// Set the validator's number method to handle comma as a decimal separator
	$.validator.methods.number = function (value, element) {
		return this.optional(element) || !isNaN(Globalize.parseNumber(value));
	};

	// Set the culture (this example uses en-US)
	Globalize.locale('en-ZA');
});