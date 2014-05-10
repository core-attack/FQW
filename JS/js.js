function check() {
    var error = "";
    try {
        var length = $("#length").val();
        try {
            $("#result").append("<p>Попытка объявления массива...</p>");
            var array = new Array(length);
            for (var i = 0; i < length; i++ ) {
                array[i] = i;
            }
            $("#result").append("<p>Объявление массива длинной " + length + " прошло успешно.</p>");
        }
        catch (e) {
            $("errors").append("<p>Ошибка объявления массива. " + e + "</p>");
        }
    }
    catch (er) {
        $("errors").append("<p>" + er + "</p>");
    }
}