$(document).ready(function () {
    console.log("Changement");
    $('#trucksList').prop('disabled', true);

    $('#chauffeursList').change(function () {
        var selectedChauffeurId = $(this).val();
        var deliveryId = $('#DeliveryId').val();

        if (selectedChauffeurId === '') {
            $('#trucksList').prop('disabled', true);
            $('#trucksList').empty();
        } else {
            $.ajax({
                headers: { RequestVerificationToken: $("#RequestCsrfToken").val() },
                url: 'GetAvailableTrucksForSelectedChauffeur',
                type: 'GET',
                data: { chauffeurId: selectedChauffeurId, deliveryId: deliveryId },
                success: function (data) {
                    $('#trucksList').empty().append($('<option>').val('').text('-- Sélectionnez un camion --'));
                    $.each(data, function (i, truck) {
                        $('#trucksList').append($('<option>').val(truck.id).text(truck.displayName));
                    });
                    $('#trucksList').prop('disabled', false);
                },
                error: function () {
                    alert('Une erreur est survenue lors de la récupération des camions disponibles.');
                }
            });
        }
    });
});