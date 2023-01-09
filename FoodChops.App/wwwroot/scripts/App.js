var PlaySound = function (fileName) {
    new Audio("/sounds/".concat(fileName, ".wav")).play();
    return "ok";
};
