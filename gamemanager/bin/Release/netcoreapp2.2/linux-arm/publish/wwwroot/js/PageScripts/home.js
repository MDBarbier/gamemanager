$(function () {
    $('.showOwnedCB').change(function () {
        var self = $(this);        
        var id = self.attr('id');
        var value = self.prop('checked');
        
        $.ajax({
            url: "../games/Home/OnlyOwned",
            data: { id: id, status: value },
            type: 'POST',
            success: function (response) {
                console.log("Successfully called server");
                window.location.replace("../games/Home");
            },
            error: function (error) {
                console.log("Error trying to call server: " + error);
            }
        });
    });
});