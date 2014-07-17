$(document).ready(function () {
    $(document).on('click', 'a.messaging', function (e) {
        e.preventDefault();
        var type = $(this).data('type');
        var id = $(this).data('for');
        $('#MessagingType').val(type);
        $('#MessagingId').val(id);
        $('#MessagingDialogTo').text($(this).data('to'));
        $('#MessagingBody').val('');
        $('#MessagingSubject').val('');
        $('#messaging-dialog').dialog('open');
    });

    $('#messaging-dialog').dialog({
        autoOpen: false,
        resizable: true,
        width: 600,
        modal: true,
        buttons: [{
            id: 'messaging-send',
            text: 'Send Message',
            click: function () {
                // replace text with contact name
                var body = $('#MessagingBody').val();
                var subject = $('#MessagingSubject').val();
                var type = $('#MessagingType').val();
                var id = $('#MessagingId').val();
                var playerId, leagueId, teamId, managerId, coachId, guardianId = null;
                switch (type) {
                    case 'Player':
                        playerId = id;
                        break;
                    case 'League':
                        leagueId = id;
                        break;
                    case 'Team':
                        teamId = id;
                        break;
                    case 'Manager':
                        managerId = id;
                        break;
                    case 'Coach':
                        coachId = id;
                        break;
                    case 'Guardian':
                        guardianId = id;
                        break;
                }
                postMessage(body, subject, leagueId, teamId, managerId, coachId, playerId, guardianId);
            }
        },
            {
            id: 'messaging-cancel',
            text: 'Cancel',
            click: function() {
                $(this).dialog('close');
            }
        }]
    });
});

function postMessage(body, subject, leagueId, teamId, managerId, coachId, playerId, guardianId) {
    var dialog = $('#messaging-dialog');
    var status = $('#messaging-status');
    var isFormValid = $('#messaging-form').valid();
    if (!isFormValid)
        return;

    $('#messaging-send').button('disable');

    var data = {
        subject: subject,
        body: body,
        playerId: playerId,
        leagueId: leagueId,
        coachId: coachId,
        guardianId: guardianId,
        managerId: managerId,
        teamId: teamId
    };

    $.ajax({
        type: 'POST',
        url: '/Messaging/SendMessage',
        data: JSON.stringify(data),
        contentType: 'application/json; charset=utf-8',
        dataType: 'json',
        success: function (result) {
            if (result) {
                if (result.Success == true) {
                    dialog.dialog('close');
                } else {
                    status.text(result.Error).removeClass('success').addClass('error').show();
                }
                $('#messaging-send').button('enable');
            }
        },
        error: function (xhr, status, error) {
            // show error
            status.text('An error occurred while trying to send the message.').removeClass('success').addClass('error').show();
            $('#messaging-send').button('enable');
        }
    });
}