/// <reference path="moment.js" />
/// <reference path="jquery-1.10.2.js" />
$(document).ready(function () {
    $('#btnSave').hide();
    //add results to table and chart
    function populateResults(data) {
            $('#tblResults tbody').html('');
            $('#bars').html('');
            $.each(data.Pages, function (index, value) {
                $('#tblResults tbody').append("<tr><td>" + (index+1) + "</td><td>" + value.URL + "</td><td>" + value.MinResponseTime.toFixed(3) + "</td><td>" + value.MaxResponseTime.toFixed(3) + "</td></tr>");
                var maxTime = data.MaxTime;

                var minPercent = value.MinResponseTime / maxTime * 100;
                var maxPercent = value.MaxResponseTime / maxTime * 100 - minPercent;
                $('#bars').append("<span>" + value.URL + "</span><div class='progress'><div class='progress-bar ' style='width:" + minPercent + "%'>" + value.MinResponseTime.toFixed(3) + " ms</div> <div class='progress-bar progress-bar-warning' style='width:" + maxPercent + "%'></div></div>");
            });
            if (data.IsScanDone == false) {
                setTimeout(callForResult, 1000 * 1.5);
            } else {
                $('#btnEval').val("Eval");
                $('#btnSave').show();
                $('#btnEval').prop("disabled", false);

            }        
    }
    //get result during processing
    function callForResult() {
        $.ajax({
            url: '/Home/GetSiteMap',
            dataType: "json",
            type: "GET",
            contentType: 'application/json; charset=utf-8',
            cache: false,
            success: function (data) {
                if (data.Pages !== null) {
                    populateResults(data);
                }
                else {
                    $('#btnEval').val("Eval");
                    $('#btnEval').prop("disabled", false);
                    $('#btnSave').show();
                }
            },
            error: function (xhr) {
                $('#btnEval').val("Eval");
                $('#btnEval').prop("disabled", false);
            }
        });
    };

    //update history
    function updateHistory() {
        $('#historyBody').html('');
        $.ajax({
            url: '/Home/GetEvaluations',
            type: "GET",
            dataType: "json",
            contentType: 'application/json; charset=utf-8',
            cache: false,
            success: function (data) {
                $.each(data, function (index, value) {
                    var created = moment(value.Created).format('DD/MM/YYYY HH:mm:ss');
                    $('#historyBody').append("<div class='list-group-item' name='historyEntry' ><input type='hidden' name='evaluationId'  value='" + value.Id + "' /><span>" + value.InitialURL + " " + created + "</span></div>");
                });
                $('[name=historyEntry]').click(function () {
                    $('#errors').html('');
                    $.ajax({
                        url: '/Home/GetEvaluationResults',
                        dataType: "json",
                        type: "GET",
                        contentType: 'application/json; charset=utf-8',
                        cache: false,
                        data: { 'evaluationId': $(this).find('[name=evaluationId]').val() },
                        success: function (data) {
                            
                            if (data.Pages !== null) {
                                populateResults(data);
                                $('#btnSave').hide();
                            }
                        },
                        error: function (xhr) {
                        }
                    });
                });
            },
            error: function (xhr) {
            }
        });
    }

    $('#btnEval').click(function () {
        $('#btnSave').hide();
        $('#tblResults tbody').html('');
        $('#bars').html('');
        $('#errors').html('');
        $.ajax({
            url: '/Home/StartScanning',
            type: "GET",
            contentType: 'application/json; charset=utf-8',
            cache: false,
            data: { url: $('#txtAddress').val() },
            success: function (data) {
                $('#btnEval').val("In process...");
                $('#btnEval').prop("disabled", true);
                setTimeout(callForResult, 1000 * 1.5);
            },
            error: function (xhr) {
                $('#errors').html('Incorrect URL');
                $('#btnSave').hide();
                $('#btnEval').val("Eval");
                $('#btnEval').prop("disabled", false);
            }
        });
    });


    $('#btnSave').click(function () {
        $('#btnSave').hide();
        $.ajax({
            url: '/Home/SaveResults',
            type: "GET",
            contentType: 'application/json; charset=utf-8',
            cache: false,
            success: function (data) {
                updateHistory();
            },
            error: function (xhr) {
            }
        });
    });

    var historyOpening = true;
    $('#historyHeader').click(function () {
        if (historyOpening) {
            updateHistory();
        }
        historyOpening ^= true;
    });


});
