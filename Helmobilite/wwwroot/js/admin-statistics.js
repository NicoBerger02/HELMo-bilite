$(document).ready(function () {

	$("#SearchClient").autocomplete({
		source: function (request, response) {
			$.ajax({
				headers: { RequestVerificationToken: $("#RequestCsrfToken").val() },
				datatype: 'json',
				url: 'SearchClientEnterprise',
				data: { searchEnterprise: request.term },
				success: function (data) { response(data) }
			})
		}
	});

	$("#SearchChauffeur").autocomplete({
		source: function (request, response) {
			$.ajax({
				headers: { RequestVerificationToken: $("#RequestCsrfToken").val() },
				datatype: 'json',
				url: 'SearchChauffeurName',
				data: { searchName: request.term },
				success: function (data) { response(data) }
			})
		}
	});

	$('#DeleteClient').click(function () {
		$('#SearchClient').val('')
	});

	$('#DeleteChauffeur').click(function () {
		$('#SearchChauffeur').val('')
	});

	$('#DeleteDate').click(function () {
		$('#SearchDate').val('')
	});

	$('#Search').click(function () {
		var filterClient = $('#SearchClient').val();
		var filterChauffeur = $('#SearchChauffeur').val();
		var filterDate = $('#SearchDate').val();

		var url = 'Statistics' + '?filterClient=' + encodeURIComponent(filterClient) + '&filterChauffeur=' + encodeURIComponent(filterChauffeur) + '&filterDate=' + encodeURIComponent(filterDate);
		window.location.href = url;
	});
});