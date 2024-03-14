window.downloadFile = function (data, mimeType, filename) {
    var blob = new Blob([data], { type: mimeType });
    var url = URL.createObjectURL(blob);
    var a = document.createElement('a');
    a.href = url;
    a.download = filename;
    a.click();
}

function updateProgressBar(progress) {
    var progressBar = document.getElementById("progressBar");
    progressBar.value = progress;
}