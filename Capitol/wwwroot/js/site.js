(function ($) {

    "use strict";
    $('nav .dropdown').hover(function () {
        var $this = $(this);
        $this.addClass('show');
        $this.find('> a').attr('aria-expanded', true);
        $this.find('.dropdown-menu').addClass('show');
    }, function () {
        var $this = $(this);
        $this.removeClass('show');
        $this.find('> a').attr('aria-expanded', false);
        $this.find('.dropdown-menu').removeClass('show');
    });
})(jQuery);



async function FetchApi(url) {
    let data = await fetch(url).then(data => data.json())
    return data;
}

setInterval(() => {
    //let currencyObj = { EURUSD, EURTRY, USDTRY, USDEUR, TRYUSD, TRYEUR }

    //FetchApi("https://api.freecurrencyapi.com/v1/latest?apikey=hMs7VHDlORmCNHdg8xfS6OHysghfly2gVXHPJ4o7&base_currency=USD&currencies=TRY,EUR").then(result => {
    //    currencyObj.USDEUR = result.data.EUR;
    //    currencyObj.USDTRY = result.data.TRY;
    //})
    //FetchApi("https://api.freecurrencyapi.com/v1/latest?apikey=hMs7VHDlORmCNHdg8xfS6OHysghfly2gVXHPJ4o7&base_currency=EUR&currencies=TRY,USD").then(result => {
    //    currencyObj.EURTRY = result.data.USD;
    //    currencyObj.EURUSD = result.data.TRY;
    //})
    //FetchApi("https://api.freecurrencyapi.com/v1/latest?apikey=hMs7VHDlORmCNHdg8xfS6OHysghfly2gVXHPJ4o7&base_currency=TRY&currencies=USD,EUR").then(result => {
    //    currencyObj.TRYEUR = result.data.EUR;
    //    currencyObj.TRYUSD = result.data.USD;
    //})
    //fetch("https://localhost:7121/Visitor/SaveCurrencyValues", {
    //    method: "POST",
    //    mode: "cors",
    //    cache: "no-cache",
    //    credentials: "same-origin",
    //    headers: {
    //        "Content-Type": "application/json",
    //    },
    //    redirect: "follow",
    //    referrerPolicy: "no-referrer",
    //    body: JSON.stringify(currencyObj),
    //})
}, (1000 * 60 * 60 * 2))

