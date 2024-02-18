

$(document).ready(function () {
    $("#categoryDropdown").change(function () {
        var categoryId = $(this).val();
        if (categoryId !== "") {
            $.ajax({
                url: "/Request/GetSubjectsByCategory",
                type: "GET",
                data: { categoryId: categoryId },
                success: function (data) {
                    $("#subjectDropdown").prop("disabled", false).html(data);
                },
                error: function () {
                    // Handle error
                }
            });
        } else {
            $("#subjectDropdown").prop("disabled", true).html("<option value=''>Select a subject</option>");
        }
    });
});