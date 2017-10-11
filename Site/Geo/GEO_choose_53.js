logger_url = "../logger/logger.py"
bgu_url = "http://hdm.ise.bgu.ac.il/GEO-3/GDMVisualizationTestPage.html"
experiment ="GEO-chooseExp"

var exper = {} //TODO integraate exper in E
E = {}
E.startTime = 0
E.endTime = 0

//Drawing solutions order
E.order = ["#1", "#2","#3","#4", "#5","#6"];
E.numOfQ=5;
E.chosenOrder = -1 
E.vote = 0


var counter;


$(document).ready(function() {

	initialize_experiment();


	onContinue();
});

function enableFrame() {
    var node = document.getElementById("d");
    if (node != null) {
        if (node.parentNode) {
            node.parentNode.removeChild(node);
        }
    }
}

function getRandomInt (min, max) {
		var num=Math.floor(Math.random() * (max - min + 1)) + min;
		if (num<3) {
			num=0;
		}
		else{
			num=3;
		}
    return num;
}



function onCheckbox() {
	if($("#consentagree").prop("checked")) {
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

	$("#btnContinue").attr('disabled', 'disabled');
	$('#progress').hide();
	

	

	E.userid = initialize_userid();
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

function showCode() {

	$(".code").text(E.userid);

}

function finalizeExperiment() {
	
	return;
	var timems = time();
	exper.endTime = timems;
	exper.timelen = exper.endTime - exper.startTime;


	ivhook_duscore();
	ivhook_dbonus();
	
	var jsonlog = JSON.stringify({
		endTime : exper.endTime,
		timelen : exper.timelen
	});

	servlog('debug', 'ExperimentEnd', jsonlog);
	servlog('exper', 'ExperimentEnd', jsonlog);
	var mlog = "exper-end" + ", " + exper.endTime;
	mouselog(mlog);
	mouselog_flush();
}





function show_page_real()
{
	$("#real.page").show()		
}

function show_page_final(){
			$("#final.page").show()	

	$("#btnContinue").hide()
	showCode();	
}

function submit_demographics() {
	var gender=document.getElementById("gender").options[document.getElementById("gender").selectedIndex].value;
	var education=document.getElementById("education").options[document.getElementById("education").selectedIndex].value;
	var age=document.getElementById("age").value;

	servlog("gender", gender);
	servlog("education", education);
	servlog("age", age);
}

function submit_quiz() {
	var q1='';
	var q2='';
	var q3 = '';
	var q4 = '';
	var passed = false;

	for (var i = 0;i < document.getElementsByName("quiz1").length; i++)
	{
		if (document.getElementsByName("quiz1")[i].checked)
		 	q1 = document.getElementsByName("quiz1")[i].value;
	}
	for (var i = 0;i < document.getElementsByName("quiz2").length; i++)
	{
		if (document.getElementsByName("quiz2")[i].checked)
		 	q2 = document.getElementsByName("quiz2")[i].value;
	}
	for (var i = 0;i < document.getElementsByName("quiz3").length; i++)
	{
		if (document.getElementsByName("quiz3")[i].checked)
		 	q3 = document.getElementsByName("quiz3")[i].value;
	}
	for (var i = 0; i < document.getElementsByName("quiz4").length; i++) {
	    if (document.getElementsByName("quiz4")[i].checked)
	        q4 = document.getElementsByName("quiz4")[i].value;
	}
	if (q1 == 'c' && q2 == 'a' && q3 == 'b' && q4 == 'a') {
	    passed = true
	  
	}
	
	
	//servlog("quiz1", q1);
	//servlog("quiz2", q2);
	//servlog("quiz3", q3);
	//servlog("quiz4", q4);
	servlog("passedQuiz", passed);
	
	if (passed == false)
	{
		alert("Sorry, you did not pass the quiz. You will now be shown the tutorial again, when you're ready to re-take the quiz click continue.");
		onContinue.curPage = 2;
		onContinue();
	}
}

function submit_solution() {
	

	//Is it needed Here?
    var explanation = document.getElementById("explanationVoteOwn").value;

	var q1 = '';
	for (var i = 0; i < document.getElementsByName("ans").length; i++) {
	    if (document.getElementsByName("ans")[i].checked)
	        q1 = document.getElementsByName("ans")[i].value;
	}

	servlog("choose", q1);
	//Text Area for solution strategy
	var solution = document.getElementById("solution").value;
	servlog("explanation", explanation);
	servlog("strategy", solution);
}
var count = 300;

function timer() {
    count = count - 1;
    if (count <= 0) {
        clearInterval(counter);
        disableIframe();
        alert("your time is out, please submit your solution.");

        return;
    }

    document.getElementById("timer").innerHTML = count + " secs"; // watch for spelling
}




function check_vote()
{
	for (var j = 1 ; j <= E.numOfQ; j++) {
			var q1="";
		for (var i = 0;i < document.getElementsByName("q"+j+"_"+E.chosenOrder).length; i++)
		{
			if (document.getElementsByName("q"+j+"_"+E.chosenOrder)[i].checked)
			 	q1 = document.getElementsByName("q"+j+"_"+E.chosenOrder)[i].value;
		}

		if (q1=="")
		{
		alert("Please vote for question number"+j+" .");
				onContinue.curPage = 7;
				onContinue();
		}
	}
	
}

var Sol;

function suggest_solution(){

    var page = E.order[E.chosenOrder];
	$(page).show()

}


function log_vote(){	
	check_vote();
	var timeVote = E.endTime - E.startTime
	for (var j = 1 ; j <= E.numOfQ; j++) {
		var q1="";	
		for (var i = 0;i < document.getElementsByName("q"+j+"_"+E.chosenOrder).length; i++)
		{
			if (document.getElementsByName("q"+j+"_"+E.chosenOrder)[i].checked)
			 	q1 = document.getElementsByName("q"+j+"_"+E.chosenOrder)[i].value;
		}
		servlog("q"+j,q1);
	}
}
	




function disableIframe()
{
      var iframe = document.getElementsByTagName('iframe')[0];
  
      d = document.createElement('iframe');

      d.style.width = iframe.offsetWidth + 'px';


        d.style.height = iframe.offsetHeight + 'px' ;

        d.style.top = iframe.offsetTop + 'px';

        var left = iframe.offsetLeft - 130;

        d.style.left = left + 'px';

        d.style.position = 'absolute';
	
		// d.onclick="event.cancelBubble = true;"

        d.style.opacity = '0';

        d.style.filter = 'alpha(opacity=0)';

		d.style.display="block";
		
        d.style.background = 'black';
        d.style.zIndex = '100';
        d.id = 'd';
        d.tagName = 'd'; 
      
        iframe.offsetParent.appendChild(d);
        // iframe.onclick = "return false;";      
}



function drawOrder()
{
	if (E.chosenOrder == -1){
		E.chosenOrder = getRandomInt(0, 5);
		servlog("chosenOrder",E.chosenOrder);
	}
}

function show_group()
{
	//$("#expSol.page").show()
	if (E.chosenOrder ==-1){
		E.chosenOrder = getRandomInt(0, 5);
		servlog("chosenOrder",E.chosenOrder);
	}
}

function onContinue() {

	if( typeof onContinue.curPage == 'undefined')
		onContinue.curPage = 0;
	onContinue.curPage++;

	//blank all pages
	$(".page").hide();
	var lentrial = 10;
	var lenfull = 60;	$("#solOwnExp").hide()

	switch(onContinue.curPage) {

		case 1:
		    $("#consent.page").show()
			break;


		case 2:
			startLog();

			$("#demographics.page").show()
			break;

		case 3:
			submit_demographics();
			E.startTime=msTime();
			$("#instructions.page").show()
			$("#btnContinue").html('Continue to quiz')

			break;

		case 4:
			E.endTime=msTime()
			var timeInstructions = E.endTime-E.startTime
			servlog("timeInstructions", timeInstructions);
			$("#btnContinue").html('Continue')
			E.startTime=msTime();
			document.getElementById("quizDiv").scrollTop=0;
			$("#quiz.page").show()
			
			break;

		case 5:
		
			E.endTime=msTime();
			var timeQuiz = E.endTime-E.startTime
			servlog("timeQuiz", timeQuiz);
			submit_quiz();
			if (onContinue.curPage==5)
			{
				urlForVC = bgu_url+'?workerId='+E.userid
				document.getElementById('bguFrame').src = urlForVC
				E.startTime=msTime();
				
				/* var voteDiv = document.getElementById('vote')
				voteDiv.style.display = 'none'; 
				var groupDiv = document.getElementById('groupExp')
				groupDiv.style.display = 'none'; 
				var groupSolsDiv = document.getElementById('groupSols')
				groupSolsDiv.style.display = 'none'; 
				var voteDescDiv = document.getElementById('voteDesc')
				voteDescDiv.style.display = 'none'; 
				var yourSolDiv = document.getElementById('yourSol')
				yourSolDiv.style.display = 'none';  */
	//			$("#btnContinue").html('Continue')

				document.getElementById("expDesc").scrollTop=0; 
				$("#experiment.page").show()
				$("#choose.page").show()	
	       		$("#expDesc.page").show()
                counter = setInterval(timer, 1000); //1000 will  run it every 1 second
                $("#ownSol.page").hide()
                $("#voteDescC.page").hide()
                $("#solOwnExp.page").hide()
                $("#btnContinue").html('Continue')
				
			}

			break;
	    case 6:
	        $("#experiment.page").show()
	      $("#choose.page").show()	
	       		$("#expDesc.page").show()
	        $("#explainSol.page").show()
	        $("#shortExp.page").hide()
	        disableIframe()
	        clearInterval(counter);
	        E.endTime = msTime();
	        var timeSolution = E.endTime - E.startTime
	        servlog("timeSolution", timeSolution);
	        //Draw how the solution pages will be ordered
	        drawOrder();
	        $("#btnContinue").html('Continue')
	        break;
		case 7:
			$("#shortExp.page").show()
			
		    // show_page_real();
		  //  servlog("timer", count);
		 //   clearInterval(counter);
			
		//	servlog("order",E.order);
			
			//How is the solved graph submitted?
			submit_solution();
			
			show_group()
			break;
		case 8:
			enableFrame()
			$("#explan.page").show()
	        $("#experiment.page").show()
	        $("#choose.page").hide()
	       	$("#expDesc.page").hide()


			E.startTime = msTime();
			suggest_solution();
			break;
			
	    case 9:
	        E.endTime = msTime()
	        log_vote();
			if (onContinue.curPage==9)
			{
				saveTheLog();
				finalizeExperiment();
				show_page_final()
			}
			
			break;
	}
}


//TODO check for screen size
