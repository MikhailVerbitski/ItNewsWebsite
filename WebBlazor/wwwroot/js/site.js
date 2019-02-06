window.TagCloudStarterFunction = {
    Start: function (jsonWords) {
        words = JSON.parse(jsonWords);
        $('#keywords').jQCloud(words);
    }
}
window.Theme = {
    ChangeTheme: function (IsDefault) {
        if (IsDefault) {
            $("body").css("background-color", "#C1BBA8");
            $("#navbar").css("background-color", "#EFE7CC");
        } else {

            $("body").css("background-color", "#000000");
            $("#navbar").css("background-color", "#276273");
        }
    }
}

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