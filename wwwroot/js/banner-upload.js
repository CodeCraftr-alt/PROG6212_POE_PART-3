const banner = document.querySelector("[data-banner-upload]");

if (banner) {
    banner.addEventListener("change", () => {
        const image = banner.files[0];
        const tooLarge = image && image.size > 5 * 1024 * 1024;
        banner.setCustomValidity(tooLarge ? "Choose a banner up to 5 MB, or clear this file to save without a banner." : "");
        if (tooLarge) banner.reportValidity();
    });
}
