$(document).ready(function () {
	$('.clickable').click(function () {
		var deliveryId = $(this).data('id');
		var weekOffset = $(this).data('weekoffset');
		showChauffeurDeliveryDetails(deliveryId, weekOffset);
	});
});

function showChauffeurDeliveryDetails(deliveryId, weekOffset) {
	$.ajax({
		headers: { RequestVerificationToken: $("#RequestCsrfToken").val() },
		url: 'GetChauffeurDeliveryDetails',
		type: 'GET',
		data: { deliveryId: deliveryId, weekOffset: weekOffset },
		success: function (data) {
			$('#modalPartial').html(data);
			$('#deliveryModal').modal('show');
		},
		error: function () {
			alert('Une erreur est survenue lors de la récupération des détails de la livraison.');
		}
	});
}

function getDeliverySucceededForm(deliveryId, weekOffset) {
	$.ajax({
		headers: { RequestVerificationToken: $("#RequestCsrfToken").val() },
		url: 'GetDeliverySucceededForm',
		type: 'GET',
		data: { deliveryId: deliveryId, weekOffset: weekOffset },
		success: function (data) {
			$('#formPartial').html(data);
		},
		error: function () {
			alert('Une erreur est survenue lors de la récupération du formulaire.');
		}
	});

}

function getDeliveryFailedForm(deliveryId, weekOffset) {
	$.ajax({
		headers: { RequestVerificationToken: $("#RequestCsrfToken").val() },
		url: 'GetDeliveryFailedForm',
		type: 'GET',
		data: { deliveryId: deliveryId, weekOffset: weekOffset },
		success: function (data) {
			$('#formPartial').html(data);
		},
		error: function () {
			alert('Une erreur est survenue lors de la récupération du formulaire.');
		}
	});

}