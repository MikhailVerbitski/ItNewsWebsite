window.TagCloudStarterFunction = {
    Start: function (jsonWords) {
        words = JSON.parse(jsonWords);
        $('#keywords').jQCloud(words);
    }
}