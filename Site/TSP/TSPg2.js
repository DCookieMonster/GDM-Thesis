logger_url = "../logger/logger.py"
bgu_url = "http://hdm.ise.bgu.ac.il/GDM.TSP.Web/GDMVisualizationTestPage.html"
experiment ="TSP"

var exper = {} //TODO integraate exper in E
E = {}
E.startTime = 0
E.endTime = 0

//Drawing solutions order
E.order = [
        ["#solA", "#solB", "#ownSol"],
        ["#solB", "#solA", "#ownSol"],
        ["#solA", "#solB", "#ownSol"],
        ["#solA","#solB","#ownSol"],
        ["#solB", "#solA", "#ownSol"],
        ["#solB","#solA","#ownSol"]
        ];
E.chosenOrder = -1 
E.vote = 0




$(document).ready(function() {

	initialize_experiment();


	onContinue();
});



function getRandomInt (min, max) {
    return Math.floor(Math.random() * (max - min + 1)) + min;
}



function onCheckbox() {
	if($("#consentagree").prop("checked")) {
		$("#btnContinue").removeAttr('disabled');

	} else {
		$("#btnContinue").removeAttr('disabled');
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
	

	

	E.userid = initialize_userid();
	// servlog("new_user", E.userid)
}

function run_block(btype, accu) {



}

var counter;
var count = 10;

function timer() {
    count = count - 1;
    if (count <= 0) {
        clearInterval(counter);
        disableIframe();
        alert("your time is out, please submit your solution.");
        servlog("timer", count);
        return;
    }

    document.getElementById("timer").innerHTML = count + " secs"; // watch for spelling
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
	var q3='';
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
	
	if (q1=='b' && q2=='a' && q3=='a')
		passed = true
	
	
	servlog("quiz1", q1);
	servlog("quiz2", q2);
	servlog("quiz3", q3);
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
	
	//Text Area for solution strategy
	var solution = document.getElementById("solution").value;
	servlog("explanation", explanation);
	servlog("strategy", solution);
}

function submit_vote()
{
	var solVote ='notChosen'; 
	
	for (var i = 0;i < document.getElementsByName("vote").length; i++)
	{
		if (document.getElementsByName("vote")[i].checked)
		{
		 	solVote = document.getElementsByName("corrIncorr")[i].value;
		 	if (solVote=='turker')
			servlog("vote", "self");
			else if ((E.order==0 && solVote=='B') || (E.order==1 && solVote=='A'))
				servlog("vote", "correct");
			else 
			{
				servlog("vote", "incorrect");	
				
			}
		 }
	}
	if (solVote != 'notChosen')
	{
		E.vote=0
		var explanation = document.getElementById("explanationVote").value;
		servlog("voteExplanation", explanation);
	}
	else
	{
		alert("Please vote for a solution.");
		E.vote=-1
	}
	
}


function suggest_solution(num) {
	var page = E.order[E.chosenOrder][num];
	document.getElementById("ownSol").scrollTop=0;
	document.getElementById("solA").scrollTop=0;
	document.getElementById("solB").scrollTop=0;
	
	$(page).show()
	
	if (page=="#ownSol")
	{
	    $("#voteDescC").show()
	    $("#experiment.page").show()
	    disableIframe();
		$("#expDesc").hide()		
		$("#indExp").hide()
		$("#solOwnExp").show()
	}
	/*
	if (page=='#ownSol')
	{
		$("#widget-container").prependTo($("#suggest"))
	}
	else if (page=='#solA')
		$("#widget-container").prependTo($("#suggest1"))
	else if (page=='#solB')
		$("#widget-container").prependTo($("#suggest2"))*/
}



function log_vote(num){

	var timeVote = E.endTime - E.startTime
	switch(E.order[E.chosenOrder][num])
	{	
		
		case "#ownSol":
			voteOwn = getCheckedRadio("ownVote");
			if (voteOwn=="noVote")
			{
				alert("Please vote");
				onContinue.curPage = 6+num;
				onContinue();
				
			}
			else
			{
				explanationVote = $("#explanationVoteOwn").val()
				servlog("vote.own", voteOwn)
				servlog("voteOwnExp", explanationVote)
				servlog("timeVoteOwn", timeVote)
			}
			break;
		case "#solA":
			voteA = getCheckedRadio("AVote");
			if (voteA=="noVote")
			{
				alert("Please vote");
				onContinue.curPage = 6+num;
				onContinue();
				
			}
			else
			{
				explanationVote = $("#explanationVoteA").val()
				servlog("vote.A", voteA)
				servlog("voteAExp", explanationVote)
				servlog("timeVoteA", timeVote)
			}
			break;
		case "#solB":
			voteB = getCheckedRadio("BVote");
			if (voteB=="noVote")
			{
				alert("Please vote");
				onContinue.curPage = 6+num;
				onContinue();
				
			}			
			else
			{
				explanationVote = $("#explanationVoteB").val()
				servlog("vote.B", voteB)
				servlog("voteBExp", explanationVote)
				servlog("timeVoteB", timeVote)
			}
			break;
	}
	
}



function disableIframe()
{
      var iframe = document.getElementsByTagName('iframe')[0];
  
      d = document.createElement('iframe');

      var width = iframe.offsetWidth;

        d.style.width = width + 'px';

        var height = iframe.offsetHeight ;

        d.style.height = height + 'px';

        var top = iframe.offsetTop;

        d.style.top = top + 'px';

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

function enableFrame() {
    var node = document.getElementById("d");
    if (node != null) {
        if (node.parentNode) {
            node.parentNode.removeChild(node);
        }
    }
}


function drawOrder()
{
	if (E.chosenOrder == -1)
		E.chosenOrder = getRandomInt(0, 5);
}

function show_group()
{
	$("#group.page").show()
	if (E.chosenOrder ==-1)
		E.chosenOrder = getRandomInt(0, 5);
}

function onContinue() {

	if( typeof onContinue.curPage == 'undefined')
		onContinue.curPage = 1;
	onContinue.curPage++;

	//blank all pages
	$(".page").hide();

	var lentrial = 10;
	var lenfull = 60;


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
				
				document.getElementById("expDesc").scrollTop=0; 
				$("#experiment.page").show()
				$("#voteDescC").hide()
				counter = setInterval(timer, 1000); //1000 will  run it every 1 second
				$("#ownSol.page").hide()				
				$("#solOwnExp.page").hide()				
				$("#ownSol.page").hide()				
				//Draw how the solution pages will be ordered
				drawOrder();
			}

			break;

		case 6:
			
		    // show_page_real();
		    E.endTime = msTime();
		    clearInterval(counter);
		   // enableFrame();
		    var timeSolution = E.endTimes - E.startTime
		    servlog("timeSolution", timeSolution);
		  //  servlog("Sol.Time.sta",E.startTime);
		    //servlog("Sol.Time.end", E.endTime);
			servlog("order",E.order);
			
			//How is the solved graph submitted?
			submit_solution();
			
			show_group()
			break;
		case 7:
			
			E.startTime = msTime();
			suggest_solution(0);
			break;
						
			/* var expDescDiv = document.getElementById('expDesc')
			expDescDiv.style.display = 'none'; 
			var indExpDiv = document.getElementById('indExp')
			indExpDiv.style.display = 'none'; 
			var voteDiv = document.getElementById('vote')
			voteDiv.style.display = 'inherit'; 
			var groupDiv = document.getElementById('groupExp')
			groupDiv.style.display = 'inherit'; 
			var groupSolsDiv = document.getElementById('groupSols')
			groupSolsDiv.style.display = 'inherit'; 
			var voteDescDiv = document.getElementById('voteDesc')
			voteDescDiv.style.display = 'inherit'; 
			var yourSolDiv = document.getElementById('yourSol')
			yourSolDiv.style.display = 'inherit'; 
			$("#experiment.page").hide()
			document.getElementById("voteDesc").scrollTop=0;
			
			$("#experiment.page").show() */
			disableIframe();
			//E.startTime=msTime();	
			break;
			
		case 8:
			E.endTime = msTime()
			log_vote(0);		
			
			/* var expDescDiv = document.getElementById('expDesc')
			expDescDiv.style.display = 'none'; 
			var indExpDiv = document.getElementById('indExp')
			indExpDiv.style.display = 'none'; 
			var voteDiv = document.getElementById('vote')
			voteDiv.style.display = 'inherit'; 
			var groupDiv = document.getElementById('groupExp')
			groupDiv.style.display = 'inherit'; 
			var groupSolsDiv = document.getElementById('groupSols')
			groupSolsDiv.style.display = 'inherit'; 
			var voteDescDiv = document.getElementById('voteDesc')
			voteDescDiv.style.display = 'inherit'; 
			var yourSolDiv = document.getElementById('yourSol')
			yourSolDiv.style.display = 'inherit'; 
			$("#experiment.page").hide()
			document.getElementById("voteDesc").scrollTop=0;
			
			$("#experiment.page").show() */
			
			//What's it for?!
			//disableIframe();
			if (onContinue.curPage==8)
			{
				E.startTime = msTime();
				suggest_solution(1);
			}
			
			break;
		case 9:
			E.endTime = msTime()
			log_vote(1);	
			if (onContinue.curPage==9)
			{
				E.startTime=msTime();	
				suggest_solution(2); 
			}
			
			break;
		case 10:
			E.endTime = msTime()
			log_vote(2);
			
			
			
			// show_page_real();
			/* E.endTime=msTime();
			var timeVote = E.endTime-E.startTime
			servlog("timeVote", timeVote);
			submit_vote();
			if (E.vote==-1)
			{
				document.getElementById("voteDesc").scrollTop=0;
				$("#experiment.page").show()
				onContinue.curPage=6;
				return;
			} */
			
			if (onContinue.curPage==10)
			{
				finalizeExperiment();
				show_page_final()
			}
			
			break;
	}
}


//TODO check for screen size
