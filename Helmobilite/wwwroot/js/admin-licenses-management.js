function getChauffeurLicensesForm(chauffeurId) {
	$.ajax({
		headers: { RequestVerificationToken: $("#RequestCsrfToken").val() },
		url: 'GetLicensesModalForm',
		type: 'GET',
		data: { chauffeurId: chauffeurId },
		success: function (data) {
			$('#modalPartial').html(data)
			$('#licensesModal').modal('show')
			$("#license_chauffeur_CE").change(function () {
				if ($(this).prop("checked")) {
					$("#license_chauffeur_C").prop("checked", true)
				}
			})

			$("#license_chauffeur_C").change(function () {
				if ($("#license_chauffeur_CE").prop("checked")) {
					$("#license_chauffeur_C").prop("checked", true)
				}

			})
		},
		error: function () {
			alert('Une erreur est survenue lors de la récupération des détails de la livraison.');
		}
	});
}