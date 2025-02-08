function convertirInputDecimal(elementoInput) {
    return parseFloat(elementoInput.value).toFixed(2);
}

function convertirValorDecimal(valor) {
    return parseFloat(valor).toFixed(2);
}

function quitarFormatoMoneda(valor) {
    valor = valor.replace("$", "").replace(",", "");
    valor = convertirValorDecimal(valor);
    return valor;
}

function agregarFormatoMoneda(valor) {
    let localidad = 'en-US';
    let opciones = {
        style: 'currency',
        currency: 'USD',
        minimumFractionDigits: 2,
        maximumFractionDigits: 2
    };
    let formatoMoneda = new Intl.NumberFormat(localidad, opciones);
    let formatoValor = formatoMoneda.format(valor);
    return formatoValor;
}