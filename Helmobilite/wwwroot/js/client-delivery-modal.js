function showDeliveryDetails(deliveryId) {
	$.ajax({
		headers: { RequestVerificationToken: $("#RequestCsrfToken").val() },
		url: 'GetClientDeliveryDetails',
		type: 'GET',
		data: { deliveryId: deliveryId },
		success: function (data) {
			$('#modalPartial').html(data);
			$('#detailsModal').modal('show');
		},
		error: function () {
			alert('Une erreur est survenue lors de la récupération des détails de la livraison.');
		}
	});
}