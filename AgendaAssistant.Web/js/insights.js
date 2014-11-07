(function () {
    window.appInsights = { queue: [], applicationInsightsId: null, accountId: null, appUserId: null, configUrl: null, start: function (n) { function u(n) { t[n] = function () { var i = arguments; t.queue.push(function () { t[n].apply(t, i) }) } } function f(n, t) { if (n) { var u = r.createElement(i); u.type = "text/javascript"; u.src = n; u.async = !0; u.onload = t; u.onerror = t; r.getElementsByTagName(i)[0].parentNode.appendChild(u) } else t() } var r = document, t = this, i; t.applicationInsightsId = n; u("logEvent"); u("logPageView"); i = "script"; f(t.configUrl, function () { f("//az416426.vo.msecnd.net/scripts/ai.0.js") }); t.start = function () { } } };
}());

//var appInsights = window.appInsights || function (config) {
//    function s(config) { t[config] = function () { var i = arguments; t.queue.push(function () { t[config].apply(t, i) }) } } var t = { config: config }, r = document, f = window, e = "script", o = r.createElement(e), i, u; for (o.src = config.url || "//az416426.vo.msecnd.net/scripts/a/ai.0.js", r.getElementsByTagName(e)[0].parentNode.appendChild(o), t.cookie = r.cookie, t.queue = [], i = ["Event", "Exception", "Metric", "PageView", "Trace"]; i.length;) s("track" + i.pop()); return config.disableExceptionTracking || (i = "onerror", s("_" + i), u = f[i], f[i] = function (config, r, f, e, o) { var s = u && u(config, r, f, e, o); return s !== !0 && t["_" + i](config, r, f, e, o), s }), t
//}({
//    instrumentationKey: "a3757b7b-3be7-4f0d-a754-e82a46c8dd9c"
//});