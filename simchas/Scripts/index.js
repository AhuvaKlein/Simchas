$(() => {

    $("#new-simcha").on('click', function () {

        $("#addSimchaModal").modal('show');

    });

    $("#new-contributor").on('click', function () {

        $("#addContributorModal").modal('show');

    });

    $(".deposit").on('click', function () {

        const button = $(this);
        const id = button.data('id');
        $("#contributorId").val(id);

        $("#addDepositModal").modal('show');

    });

    $(".editContributor").on('click', function () {

        const button = $(this);
        const firstName = button.data('firstname');
        const lastName = button.data('lastname');
        const id = button.data('id');
        const cellNumber = button.data('cellnumber');
        const dateJoined = button.data('datejoined');
        const alwaysJoin = button.data('alwaysjoin');
        console.log(id);

        $("#firstName").val(firstName);
        $("#lastName").val(lastName);
        $("#contributorIdModal").val(id);
        $("#cellNumber").val(cellNumber);
        $("#dateJoined").val(dateJoined);
        if (alwaysJoin == "True") {
            $("#alwaysJoin").prop('checked', true);
        }
        else {
            $("#alwaysJoin").prop('checked', false);
        }


        $("#editContributorModal").modal('show');

    });

})