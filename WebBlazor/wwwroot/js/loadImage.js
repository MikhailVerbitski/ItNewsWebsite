function previewFile() {
    var preview = document.querySelector('file'); //selects the query named img
    var file = document.querySelector('input[type=file]').files[0]; //sames as here
    $.ajax({
        url: "/api/Post/AddImage",
        cache: false,
        contentType: false,
        processData: false,
        async: false,
        data: form_data,
        type: 'post',
        success: function (data) {
            $('#content').append("\n![comment to the image](" + data + ")");
        }
    });
}