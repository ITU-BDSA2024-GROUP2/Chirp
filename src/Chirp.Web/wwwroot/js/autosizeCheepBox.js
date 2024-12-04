document.addEventListener("DOMContentLoaded", function () {
    const messageBox = document.getElementById("messageBox");

    if (messageBox) {
        messageBox.addEventListener("input", function () {
            this.style.height = "auto"; 
            this.style.height = (this.scrollHeight) + "px"; 
        });
    }
});
