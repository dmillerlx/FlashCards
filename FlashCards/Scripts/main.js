$(document).ready(function () {
    //alert("Start");

    var currentQuestionID;

    $("#getNextRandomButton").off("click");
    $("#deleteQuestion").off("click");
    $("#showAnswer").off("click");
    $("#submitQuestionList").off("click");
    

    $("#getNextRandomButton").click(function (evt) {
        //alert("test");
        evt.preventDefault();

        $.ajax({
            url: "/api/FlashCardsRandom",
            data: null,
            dataType: "json",
            success: function (entry) {
                //debugger;
                currentQuestionID = entry.FlashCardEntryId;
                $("#question").html(entry.Question);
                $("#answer").html(nl2br(entry.Answer, false));
                $("#answer").hide();
                

            },
            error:
                function (xhr, ajaxOptions, thronError) {
                    alert(xhr.status);
                    alert(thrownError);
                }
        });      

    });

    $("#showAnswer").click(function (evt) {
        evt.preventDefault();

        $("#answer").show();
    })

    $("#deleteQuestion").click(function (evt) {
        evt.preventDefault();

        $.ajax({
            url: "/api/FlashCards/" + currentQuestionID,
            method: "DELETE",
            data: null,
            dataType: "json",
            success: function (entry) {
                alert("Successfully deleted");

                $("#question").html("");
                $("#answer").html("");
                $("#answer").hide();


            }
        });

        return false;
    })


    $("#submitQuestionList").click(function (evt) {
        evt.preventDefault();

        //var questionData = $("#questionList").serialize();// { "data": $("#questionList").val() };
        debugger;
        var questionData = { "questionList": JSON.stringify($("#questionList").val()) };

        //$.post("/api/FlashCards", questionData).done(function () {
        //    $("#questionListStatus").html("Done");
        //}).fail(function (jqXHR, textStatus, error) {
        //    console.log("Post failed:" + error);
        //    $("#questionListStatus").html(error);
        //});

        $.ajax({
            method: "POST",
            processData: false,
            url: "/api/FlashCards",
            //contentType: "application/json",
            contentType: "application/x-www-form-urlencoded; charset=utf-8",
            //data: JSON.stringify(questionData)
            //data: "=" + JSON.stringify($("#questionList").val())
            data: "=" + $("#questionList").val()
        }).done(function () {
            alert("Questions Added");
            //$("#questionListStatus").html("Done");
        }).fail(function (jqXHR, textStatus, error) {
            console.log("Post failed:" + error);
            $("#questionListStatus").html(error);
        });

    });


    function nl2br(str, is_xhtml) {
        var breakTag = (is_xhtml || typeof is_xhtml === 'undefined') ? '<br />' : '<br>';
        return (str + '').replace(/([^>\r\n]?)(\r\n|\n\r|\r|\n)/g, '$1' + breakTag + '$2');
    }

});