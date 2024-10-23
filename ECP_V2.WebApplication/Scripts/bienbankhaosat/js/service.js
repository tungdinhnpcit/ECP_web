
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
                var jsondata = JSON.stringify(d);
                //console.log("d.toString()" + jsondata);

                if (d.Mess.toString().indexOf("error:") >= 0) {
                    alert(d.Mess);
                    if (d.Mess.toString().indexOf("authority") >= 0) {
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
    PostAjaxt: function ExcuteAjaxt(url, data, callback, async) {
        var asy = false;
        if (async != null)
            asy = async
        $.ajax({
            ContentType: 'application/json; charset=utf-8',
            dataType: 'json',
            type: 'POST',
            url: this.apiserver + url,
            data: data,
            async: asy,                        
        }).done(function (d) {
            try {
                var jsondata = JSON.stringify(d);
                //console.log("d.toString()" + jsondata);

                if (d.Mess.toString().indexOf("error:") >= 0) {
                    alert(d.Mess);
                    if (d.Mess.toString().indexOf("authority") >= 0) {
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
