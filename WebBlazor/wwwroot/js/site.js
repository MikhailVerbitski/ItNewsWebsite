window.TagCloudStarterFunction = {
    Start: function (jsonWords) {
        words = JSON.parse(jsonWords);
        let cl = $('#keywords').clone();
        $('#keywords').jQCloud(words);
        $(window).resize(function () {
            $('#keywords').replaceWith(cl.clone());
            $('#keywords').jQCloud(words);
        })
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