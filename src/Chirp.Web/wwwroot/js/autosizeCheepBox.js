document.addEventListener("DOMContentLoaded", function () {
    const message = document.getElementById("Message");

    if (message) {
        message.addEventListener("input", function () {
            this.style.height = "auto";
            this.style.height = (this.scrollHeight) + "px";
        });
    }
});
