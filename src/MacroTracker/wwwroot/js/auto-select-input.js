document.addEventListener("focusin", (event) => {
    const target = event.target;
    if (!(target instanceof HTMLElement)) {
        return;
    }

    if (target instanceof HTMLTextAreaElement) {
        target.select();
        return;
    }

    if (!(target instanceof HTMLInputElement)) {
        return;
    }

    const selectableTypes = new Set([
        "text",
        "search",
        "url",
        "tel",
        "email",
        "password",
        "number"
    ]);

    const inputType = (target.type || "text").toLowerCase();
    if (!selectableTypes.has(inputType)) {
        return;
    }

    target.select();
});
