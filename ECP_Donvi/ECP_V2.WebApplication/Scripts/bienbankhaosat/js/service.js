
var service = {
    apiserver: "/api/bbks/",
    ExcuteAjaxt: function ExcuteAjaxt(url, data, callback, async) {
        var asy = false;
        if (async != null)
            asy = async
        $.ajax({
            type: "GET",
            url: this.apiserver + url,
            data: data,
            async: asy
        }).done(function (d) {
            try {
                if (d.toString().indexOf("error:") >= 0) {
                    alert(d);
                    if (d.toString().indexOf("authority") >= 0) {
                        window.location = "./login.aspx";
                    }

                } else {
                    if (callback != null)
                        callback(d)
                }
            } catch (e) {
                console.log(e);
            }
        });
    },

};
