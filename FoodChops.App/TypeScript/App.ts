var PlaySound = (fileName: string) => {
    new Audio(`/sounds/${fileName}.wav`).play();
    return "ok";
}