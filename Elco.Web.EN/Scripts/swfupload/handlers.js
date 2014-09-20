function preLoad() {
    if (!this.support.loading) {
        alert("你需要安装Flashplayer才能使用上传功能！");
        return false;
    }
}
function loadFailed() {
    alert("Flash加载失败，请联系网站管理员！");
}
function fileDialogComplete(numFilesSelected, numFilesQueued) {
    try {
        if (numFilesSelected > 0) {
            //document.getElementById(this.customSettings.cancelButtonId).disabled = false;
        }
        /* I can't want auto start the upload and I can do that here */
        //this.startUpload();
    } catch (ex) {
        this.debug(ex);
    }
}
function uploadProgress(file, bytesLoaded, bytesTotal) {
    try {
        var percent = Math.ceil((bytesLoaded / bytesTotal) * 100);
        document.getElementById('processbar').innerHTML = percent + "%";
        if (percent == 100) {
            document.getElementById('processbar').innerHTML = "上传已完成！";
        }
    } catch (ex) {
        this.debug(ex);
    }
}
function uploadError(file, errorCode, message) {
    try {
        switch (errorCode) {
            case SWFUpload.UPLOAD_ERROR.HTTP_ERROR:
                this.debug("Error Code: HTTP Error, File name: " + file.name + ", Message: " + message);
                break;
            case SWFUpload.UPLOAD_ERROR.UPLOAD_FAILED:
                this.debug("Error Code: Upload Failed, File name: " + file.name + ", File size: " + file.size + ", Message: " + message);
                break;
            case SWFUpload.UPLOAD_ERROR.IO_ERROR:
                this.debug("Error Code: IO Error, File name: " + file.name + ", Message: " + message);
                break;
            case SWFUpload.UPLOAD_ERROR.SECURITY_ERROR:
                this.debug("Error Code: Security Error, File name: " + file.name + ", Message: " + message);
                break;
            case SWFUpload.UPLOAD_ERROR.UPLOAD_LIMIT_EXCEEDED:
                this.debug("Error Code: Upload Limit Exceeded, File name: " + file.name + ", File size: " + file.size + ", Message: " + message);
                break;
            case SWFUpload.UPLOAD_ERROR.FILE_VALIDATION_FAILED:
                this.debug("Error Code: File Validation Failed, File name: " + file.name + ", File size: " + file.size + ", Message: " + message);
                break;
            case SWFUpload.UPLOAD_ERROR.FILE_CANCELLED:
                // If there aren't any files left (they were all cancelled) disable the cancel button
                if (this.getStats().files_queued === 0) {
                    //document.getElementById(this.customSettings.cancelButtonId).disabled = true;
                }
                break;
            case SWFUpload.UPLOAD_ERROR.UPLOAD_STOPPED:
                this.debug("Stopped");
                break;
            default:
                this.debug("Error Code: " + errorCode + ", File name: " + file.name + ", File size: " + file.size + ", Message: " + message);
                break;
        }
    } catch (ex) {
        this.debug(ex);
    }
}

