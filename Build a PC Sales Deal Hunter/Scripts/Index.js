$(document).ready(function ()
{
    var $fields = $('#fields');
    $('#btnAddPreference').click(function (e)
    {
        $("button[type=submit]").removeAttr('disabled');
        e.preventDefault();
        var rowCount = $('#fields tr').length;
        if (rowCount <= 10)
        {
            $('.typeahead').typeahead('destroy');
            $('<tr ><td><input class = "typeahead" type="text" required placeholder="Query (eg GTX 970)" name="query" /><input type="number" pattern="\d+" min="0" required placeholder="Max $ willing to pay" name="lessThan" /> <button onclick = "Delete($(this).parent().parent())" type = "button" class="btn btn-danger delete"><span class="glyphicon glyphicon-minus"></span></button><td></tr>').appendTo($fields);
            typeahead_initialize();
        }
    });

    var table = $('#dataTable').DataTable(
    {
        "paging": false,
        "info": false,
        "searching": false
    });
});

function typeahead_initialize()
{
    var substringMatcher = function (strs) {
        return function findMatches(q, cb) {
            var matches, substringRegex;
            matches = [];
            substrRegex = new RegExp(q, 'i');
            $.each(strs, function (i, str) {
                if (substrRegex.test(str)) {
                    matches.push(str);
                }
            });
            cb(matches);
        };
    };

    $('.typeahead').typeahead({
        hint: true,
        highlight: true,
        minLength: 1
    },
    {
        name: 'items',
        source: substringMatcher(items)
    });
}
typeahead_initialize();

function Delete(e)
{
    e.remove();
    if ($('#fields tr').length < 1)
        $("button[type=submit]").attr('disabled', 'disabled');
}

function RemoveEmail(_email)
{
    if (_email.length > 0)
        $.ajax({
            type: "POST",
            url: "/Home/Remove/",
            contentType: "application/json",
            data: JSON.stringify({ email: _email }),
            dataType: "json",
            success: function () {
                $('.quit').modal('toggle');
                swal("Success!", "Email Address Removed!", "success");
            },
            error: function () {
                $('.quit').modal('toggle');
                swal("Error Son", "Something went wrong!", "error");
            }
        });
    else
    {
        $('.quit').modal('toggle');
        swal("Email address required!", "We're fairly certain your email address isn't nothing.", "error");
    }
}

function IndividualTask(_email)
{
    $.ajax({
        type: "POST",
        url: "/Home/IndividualTask/",
        contentType: "application/json;",
        data: JSON.stringify({ email: _email }),
        dataType: "json",
        success: PopulateModal,
        error: function () { swal("Error Son", "Queries under this email couldn't be retrieved!", "error"); }
    });
}


function DeleteTaskItem(e)
{
    $.ajax({
        type: "POST",
        url: "/Home/DeleteIndividualTask/",
        contentType: "application/json",
        data: JSON.stringify({ email: $('#email').val(), query: e.find('.QueryText').text(), price: e.attr('data-price') }),
        dataType: "json",
        success: function () {
            e.remove();
        },
        error: function () {
            swal("Error Son", "Something went wrong!", "error");
        }
    });
}

function PopulateModal(data, status) {
    var $fields = $('#queriesBody');
    $fields.empty();
    for (var i = 0; i < data.length; i++) {
        $('<tr data-price="' + data[i].Price + '" data-query="' + $.trim(data[i].Query) + '"><td class="QueryText">' + data[i].Query + '</td><td>$' + data[i].Price + '</td>'
        + '<td align = "center"><button onclick = "DeleteTaskItem($(this).parent().parent())" type = "button" class="btn btn-danger"><span class="glyphicon glyphicon-minus-sign"></span></button></td>').appendTo($fields);
    }
    if (data.length == 0)
        $('<tr><td>No Records Found</td><td></td><td></td></tr>').appendTo($fields);
    $('.showQueries').modal('toggle');
}