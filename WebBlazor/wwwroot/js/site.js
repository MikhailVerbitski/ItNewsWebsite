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
window.DragAndDrop = {
    Init: function (id) {
        var holder = document.getElementById('textareaContent');
        if (holder != null) {
            holder.ondragover = function () { return false; };
            holder.ondragenter = function () { return false; };
            holder.ondrop = async function (e) {
                e.preventDefault();
                let reader = new FileReader();
                let mas = e.dataTransfer.files[0].name.split(".");
                let extension = '.' + mas[mas.length - 1];
                reader.readAsDataURL(e.dataTransfer.files[0]);
                reader.onload = async function () {
                    await DotNet.invokeMethodAsync("WebBlazor", "Send", reader.result.replace(/^data:(.*;base64,)?/, ''), id, extension)
                };
                reader.onerror = function (error) {
                    console.log('Error: ', error);
                };
            }
        }
    }
}