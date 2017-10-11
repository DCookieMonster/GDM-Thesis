logger_url = "../logger/logger.py"
bgu_url = "http://hdm.ise.bgu.ac.il/GDM.GEO.Web/GDMVisualizationTestPage.html"
experiment ="vertexcover"

var exper = {} //TODO integraate exper in E
E = {}
E.startTime = 0
E.endTime = 0
E.order = 0 
E.vote = 0



$(document).ready(function() {

	initialize_experiment();


	onContinue();
});




function onCheckbox() {
	if($("#consentagree").prop("checked")) {
		$("#btnContinue").removeAttr('disabled');

	} else {
		$("#btnContinue").attr('disabled', 'disabled');
	}
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
	//solving the quiz
	if (q1=='n2' && q2=='y' && q3=='n1')
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
	var explanation = document.getElementById("explanation").value;
	var solution = document.getElementById("solution").value;
	servlog("explanation", explanation);
	servlog("solution", solution);
}

function submit_vote()
{
	var solVote ='notChosen'; 
	
	for (var i = 0;i < document.getElementsByName("vote").length; i++)
	{
		if (document.getElementsByName("vote")[i].checked)
		 	solVote = document.getElementsByName("vote")[i].value;
	}
	if (solVote != 'notChosen')
	{
		if (solVote=='turker')
			servlog("vote", "self");
		else if ((E.order==0 && solVote=='B') || (E.order==1 && solVote=='A'))
			servlog("vote", "correct");
		else 
			servlog("vote", "incorrect");	
			E.vote=0
	}
	else
	{
		alert("Please vote for a solution.");
		E.vote=-1
	}
	
}

function disableIframe()
{
      var iframe = document.getElementsByTagName('iframe')[0];
  
      d = document.createElement('iframe');

        d.style.width = iframe.offsetWidth + 'px';

        d.style.height = iframe.offsetHeight + 'px';

        d.style.top = iframe.offsetTop + 'px';

        d.style.left = iframe.offsetLeft + 'px';

        d.style.position = 'absolute';
	
		// d.onclick="event.cancelBubble = true;"
        d.style.opacity = '0';

        d.style.filter = 'alpha(opacity=0)';		d.style.display="block";

        d.style.background = 'black';
        d.style.zIndex = '100';
 
        iframe.offsetParent.appendChild(d);
        // iframe.onclick = "return false;";
        
       
}

$("splitinterface-widget-container").click(function(event){
  event.stopPropagation();
  return false;
});  

function onContinue() {

	if( typeof onContinue.curPage == 'undefined')
		onContinue.curPage = 0;
	onContinue.curPage++;

	//blank all pages
	if (onContinue.curPage!=6)
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
			// submit_demographics();
			E.startTime=msTime();
			$("#instructions.page").show()
			$("#btnContinue").html('Continue to quiz')

			break;

		case 4:
			E.endTime=msTime()
			var timeInstructions = E.endTime-E.startTime
			// servlog("timeInstructions", timeInstructions);
			$("#btnContinue").html('Continue')
			E.startTime=msTime();
			document.getElementById("quizDiv").scrollTop=0;
			$("#quiz.page").show()
			
			break;

		case 5:
		
			E.endTime=msTime();
			var timeQuiz = E.endTime-E.startTime
			// servlog("timeQuiz", timeQuiz);
			// submit_quiz();
			if (onContinue.curPage==5)
			{
				urlForVC = bgu_url+'?workerId='+E.userid
				 document.getElementById('bguFrame').src = urlForVC
				E.startTime=msTime();
				var voteDiv = document.getElementById('vote')
				voteDiv.style.display = 'none'; 
				var groupDiv = document.getElementById('groupExp')
				groupDiv.style.display = 'none'; 
				var groupSolsDiv = document.getElementById('groupSols')
				groupSolsDiv.style.display = 'none'; 
				var voteDescDiv = document.getElementById('voteDesc')
				voteDescDiv.style.display = 'none'; 
				var yourSolDiv = document.getElementById('yourSol')
				yourSolDiv.style.display = 'none'; 
				document.getElementById("expDesc").scrollTop=0;
				$("#experiment.page").show()
				
				
			}

			break;

		case 6:
			
			// show_page_real();
			
			E.endTime=msTime();
			var timeSolution = E.endTime-E.startTime
			var rand = Math.random();
			if (rand<0.5)
			{
				E.order=1
				document.getElementById("solOne").src = "images/solB_c.jpg"
				document.getElementById("solTwo").src = "images/solA_c.jpg"
			}

			// servlog("timeSolution", timeSolution);
			// submit_solution();
			var expDescDiv = document.getElementById('expDesc')
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
						// disableIframe();
			$("#experiment.page").show()
			disableIframe();
			E.startTime=msTime();	
			break;
			
		case 7:

			// show_page_real();
			E.endTime=msTime();
			var timeVote = E.endTime-E.startTime
			// servlog("timeVote", timeVote);
			// submit_vote();
			if (E.vote==-1)
			{
				document.getElementById("voteDesc").scrollTop=0;
				$("#experiment.page").show()
				onContinue.curPage=6;
				return;
			}
			finalizeExperiment();
			show_page_final()		
			break;


	}
}


//TODO check for screen size
