function inputValorConvertirDecimal(valor) {
    return parseFloat(valor);
}

function inputConvertirDecimal(elementoInput) {
    let valor = inputValorConvertirDecimal(elementoInput.value);
    return valor;
}

function inputValorAgregarFormatoMoneda(valor) {
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

function inputAgregarFormatoMoneda(elementoInput) {
    let valor = inputValorQuitarFormatoMoneda(elementoInput.value.replace(/,/g, ''));
    if (isNaN(valor) || valor === undefined) {
        elementoInput.value = '$0.00';
        valor = 0;
    }
    elementoInput.value = inputValorAgregarFormatoMoneda(valor);
}

function inputValorQuitarFormatoMoneda(valor) {
    valor = valor.replace("$", "").replace(/,/g, '');
    valor = inputValorConvertirDecimal(valor);
    return valor;
}

function InputQuitarFormatoMoneda(elementoInput) {
    if (isNaN(elementoInput.value)) {
        console.log('isNaN');
        console.log(elementoInput);
        return;
    }
    if (elementoInput.value == undefined) {
        console.log('undefined');
        console.log(elementoInput);
        return;
    }
    elementoInput.value = inputValorQuitarFormatoMoneda(elementoInput.value);
}

document.querySelectorAll('.formato-numerico')
        .forEach(function (elementoInput) {
    elementoInput.addEventListener('blur', function () {
        inputAgregarFormatoMoneda(elementoInput);
    });

    elementoInput.addEventListener('focus', function () {
        InputQuitarFormatoMoneda(elementoInput);
    });

    elementoInput.addEventListener('input', function () {
        let raw = elementoInput.value.replace(/[^0-9.]/g, '');
        let parts = raw.split('.');
        if (parts.length > 2) {
            raw = parts[0] + '.' + parts[1];
        }
        if (parts[1]?.length > 2) {
            parts[1] = parts[1].substring(0, 2);
            raw = parts[0] + '.' + parts[1];
        }
        elementoInput.value = raw;
    });
});