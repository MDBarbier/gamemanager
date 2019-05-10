document.addEventListener('DOMContentLoaded', function () {
    var sep = '\uE000'; // an unusual char: unicode 'Private Use, First'

    window.addEventListener('pagehide', function (e) {
        window.name += sep + window.pageXOffset + sep + window.pageYOffset;
    });

    if (window.name && window.name.indexOf(sep) > -1) {
        var parts = window.name.split(sep);
        if (parts.length >= 3) {
            window.name = parts[0];
            window.scrollTo(parseFloat(parts[parts.length - 2]), parseFloat(parts[parts.length - 1]));
        }
    }
});