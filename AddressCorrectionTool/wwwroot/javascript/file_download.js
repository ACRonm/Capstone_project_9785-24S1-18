window.downloadFile = function (data, mimeType, filename) {

    var blob = new Blob([data], { type: mimeType });
    var url = URL.createObjectURL(blob);
    var a = document.createElement('a');
    a.href = url;
    a.download = filename;
    a.click();
}

function updateProgressBar(progress) {
    var progressDiv = document.querySelector(".progress");
    var progressBarDiv = document.querySelector(".progress-bar");
    if (progressDiv && progressBarDiv) {
        progressDiv.setAttribute('aria-valuenow', progress);
        progressBarDiv.style.width = progress + '%';

        // Check if progress is 100
        if (progress >= 100) {
            // If it is, hide the modal
            $('#myModal').modal('hide');
        }

    } else {
        console.error('Could not find the progress elements');
    }
}