var PhotoUploadCallbacks = $.Callbacks();

$(document).ready(function () {
    $('#upload-photo-dialog').dialog({
        autoOpen: false,
        resizable: true,
        modal: true,
        width: 900,
        height: 625,
        buttons: {
            'Cancel': function () {
                $(this).dialog('close');
            }
        }
    });

    $('#photo-preview').on('click', '#SubmitCrop', function (e) {
        e.preventDefault();
        var data = {
            File: $('#CropFile').val(),
            Top: $('#CropTop').val(),
            Left: $('#CropLeft').val(),
            Bottom: $('#CropBottom').val(),
            Right: $('#CropRight').val(),
            Type: $('#CropType').val(),
            Id: $('#CropId').val()
        };

        $.ajax({
            type: 'POST',
            url: '/Photos/CropPhoto',
            data: JSON.stringify(data),
            contentType: 'application/json; charset=utf-8',
            dataType: 'json',
            success: function (result) {
                if (result) {
                    if (result.Success == true) {
                        // update photo preview on page
                        $('#upload-photo-dialog').dialog('close');

                        // clear cropped values
                        $('#photo-preview').hide();

                        // call callback
                        var id = $('#CropId').val();
                        var file = result.file;
                        PhotoUploadCallbacks.fire(data.Id, data.File);
                    } else {
                        // output .Error
                    }
                }
            },
            error: function (xhr, status, error) {
                // output error
            }
        });
    });

    $('#photo-upload').on('click', '#UploadPhoto', function (e) {
        e.preventDefault();
        $('#Photo').upload(
            '/Photos/UploadPhoto',
            {},
            function (res) {
                if ((res == '') || (res == 'empty')) // plugin does not handle empty result properly
                    return;
                res = $.parseJSON(res);

                if (res.success == true) {
                    // show photo
                    if ((res.type != null) && (res.type != '')) {
                        $('#photo-preview').find('img').attr('src', '/Images/Temp/' + res.file + '.jpeg?rnd=' + new Date().getTime());
                        $('#CropFile').val(res.file);
                        $('#photo-preview').show();
                        // clear file selection
                        $('#Photo').val('');

                        jQuery(function ($) {

                            // Create variables (in this scope) to hold the API and image size
                            var jcrop_api,
                                boundx,
                                boundy,

                                // Grab some information about the preview pane
                                $preview = $('#preview-pane'),
                                $pcnt = $('#preview-pane .preview-container'),
                                $pimg = $('#preview-pane .preview-container img'),

                                xsize = $pcnt.width(),
                                ysize = $pcnt.height();


                            $('#jcrop-target').Jcrop({
                                onChange: updatePreview,
                                onSelect: updatePreview,
                                setSelect: [0, 0, 0, 0],
                                aspectRatio: xsize / ysize
                            }, function () {
                                // Use the API to get the real image size
                                var bounds = this.getBounds();
                                boundx = bounds[0];
                                boundy = bounds[1];
                                // Store the API in the jcrop_api variable
                                jcrop_api = this;

                                // Move the preview into the jcrop container for css positioning
                                $preview.appendTo(jcrop_api.ui.holder);
                            });

                            function updatePreview(c) {
                                if (parseInt(c.w) > 0) {
                                    $('#CropTop').val(c.y);
                                    $('#CropLeft').val(c.x);
                                    $('#CropBottom').val(c.y2);
                                    $('#CropRight').val(c.x2);

                                    var rx = xsize / c.w;
                                    var ry = ysize / c.h;

                                    $pimg.css({
                                        width: Math.round(rx * boundx) + 'px',
                                        height: Math.round(ry * boundy) + 'px',
                                        marginLeft: '-' + Math.round(rx * c.x) + 'px',
                                        marginTop: '-' + Math.round(ry * c.y) + 'px'
                                    });
                                }
                            };

                        });
                    }
                }
            }
        );
    });
});