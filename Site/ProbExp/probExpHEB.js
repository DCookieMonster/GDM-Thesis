logger_url = "../logger/logger.py"
experiment = "ProbExp"

var exper = {} //TODO integraate exper in E
E = {}
E.startTime = 0
E.endTime = 0

//Drawing solutions order

var counter;


$(document).ready(function () {

    initialize_experiment();


    onContinue();
});



function getRandomInt(min, max) {
    return Math.floor(Math.random() * (max - min + 1)) + min;
}



function onCheckbox() {
    if ($("#consentagree").prop("checked")) {
        $("#btnContinue").removeAttr('disabled');

    } else {
        $("#btnContinue").attr('disabled', 'disabled');
    }
}


function getCheckedRadio(radio_group_name) {
    radio_group = document.getElementsByName(radio_group_name)
    for (var i = 0; i < radio_group.length; i++) {
        var button = radio_group[i];
        if (button.checked) {
            return button.value;
        }
    }
    return "noVote";
}


function initialize_experiment() {
    $(document).ajaxError(abortAll);

    //$("#btnContinue").attr('disabled', 'disabled');
    $('#progress').hide();




    //E.userid = initialize_userid();
    // servlog("new_user", E.userid)
}

function run_block(btype, accu) {



}





function startLog() {
    return;
    var timems = time();
    exper['startTime'] = timems;

    exper['browser'] = BrowserDetect.browser;
    exper['version'] = BrowserDetect.version;
    exper['OS'] = BrowserDetect.OS;
    servlog('exper', 'ExperimentStart', JSON.stringify(exper));

    exper['agent'] = navigator.userAgent;
    exper['JQbrowser'] = $.browser;
    servlog('debug', 'ExperimentStart', JSON.stringify(exper));

    var mlog = "exper-begin" + "," + timems;
    mouselog(mlog);
}


function finalizeExperiment() {

    return;
    var timems = time();
    exper.endTime = timems;
    exper.timelen = exper.endTime - exper.startTime;


    ivhook_duscore();
    ivhook_dbonus();

    var jsonlog = JSON.stringify({
        endTime: exper.endTime,
        timelen: exper.timelen
    });

    servlog('debug', 'ExperimentEnd', jsonlog);
    servlog('exper', 'ExperimentEnd', jsonlog);
    var mlog = "exper-end" + ", " + exper.endTime;
    mouselog(mlog);
    mouselog_flush();
}





function show_page_real() {
    $("#real.page").show()
}

function show_page_final() {
    $("#final.page").show()

    $("#btnContinue").hide()
   // showCode();
}

function submit_demographics() {
     E.userid = document.getElementById("id").value;
    conlog("userid: " + E.userid)

}

var numOftry =0;

function submit_quiz() {
    var q1 = '';
    var q2 = '';
    var q3 = '';
    var q4 = '';
    var q5 = '';
    var passed = false;
    numOftry++;

    for (var i = 0; i < document.getElementsByName("quiz1").length; i++) {
        if (document.getElementsByName("quiz1")[i].checked)
            q1 = document.getElementsByName("quiz1")[i].value;
    }
    for (var i = 0; i < document.getElementsByName("quiz2").length; i++) {
        if (document.getElementsByName("quiz2")[i].checked)
            q2 = document.getElementsByName("quiz2")[i].value;
    }
    for (var i = 0; i < document.getElementsByName("quiz3").length; i++) {
        if (document.getElementsByName("quiz3")[i].checked)
            q3 = document.getElementsByName("quiz3")[i].value;
    }
     for (var i = 0; i < document.getElementsByName("quiz4").length; i++) {
        if (document.getElementsByName("quiz4")[i].checked)
            q4 = document.getElementsByName("quiz4")[i].value;
    }

 for (var i = 0; i < document.getElementsByName("quiz5").length; i++) {
        if (document.getElementsByName("quiz5")[i].checked)
            q5 = document.getElementsByName("quiz5")[i].value;
    }


    if (q1 == 'a' && q2 == 'd' && q3 == 'a'&& q4 == 'b'&& q5 == 'd') {
        passed = true

    }


    //servlog("quiz1", q1);
    //servlog("quiz2", q2);
    //servlog("quiz3", q3);
    if (passed==true) {
        servlog("quiz","passed");
    };
    //servlog("passedQuiz", passed);
    if (passed==false && numOftry==3) {
        servlog("quiz",passed);
        alert("Sorry, you did not pass the quiz. You will not take part in this experiment.");
        onContinue.curPage = 3;
        onContinue();
        return;
    };
    var num=3-numOftry;
    if (passed == false) {
        alert("Sorry, you did not pass the quiz,you have "+num+" more tires.");
        onContinue.curPage = 1;
        onContinue();
    }
    
}




function onContinue() {

    if (typeof onContinue.curPage == 'undefined')
        onContinue.curPage = 0;
    onContinue.curPage++;

    //blank all pages
    $(".page").hide();

    var lentrial = 10;
    var lenfull = 60;


    switch (onContinue.curPage) {


        case 1:
            startLog();
            $("#demographics.page").show()
            break;
            numOftry=0;

        case 2:
            submit_demographics();
            E.startTime = msTime();
             document.getElementById("quizDiv").scrollTop = 0;
            $("#quiz.page").show()
            $("#btnContinue").html('Sumbit')

            break;

        
        case 3:

            submit_quiz();

            if (onContinue.curPage == 3) {
                finalizeExperiment();
                show_page_final();
             $("#final.page").show();

            }
            break;
       case 4:
            finalizeExperiment();
            show_page_final();
            $("#final.page").hide();
            $("#notpassfinal.page").show();
            break;
    }
}


//TODO check for screen size
