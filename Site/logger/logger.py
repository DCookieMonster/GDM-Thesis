#! /usr/bin/python
import cgi, re, imp, json, time, sys
import logging
import pymssql
logger = logging.getLogger('log')


import cgitb
cgitb.enable(format='plain')



def abort404(message):
    logger.error(message)
    print 'Status: 404 Not Found %s\n' % message
    exit(-1)

def abort400(message):
    logger.error(message)
    print 'Status: 400 Bad Request %s\n' % message
    exit(-1)

def getQueryArgument(argName, query=cgi.FieldStorage()):
    if not query.has_key(argName): abort400( argName + " not specified")
    argValue = query[argName].value

    return argValue


def logEvent():
    userid = getQueryArgument("userid")
    key = getQueryArgument("key")
    value = getQueryArgument("value")
    clientTime = getQueryArgument("time")
    message = userid+','+key+','+value+','+clientTime
    logger.info(message)
    #xkv = XKeyValue(userid=userid, key=key, value=value, clientTime=clientTime)
    reply("Event logged: " + key)


def reply(status, data='nodata'):
    print "Content-Type: text/plain\n"

    msg = {'status':status, 'data':data}
    jmsg = json.dumps(msg)

    print 'jmsg'

def logdb():
    message = "dor"
    conn = pymssql.connect(host='132.72.64.53', user='gdm', password='a1234', database='GDMexp')
    cur = conn.cursor()
    cur.execute("INSERT INTO message VALUES(%s)",message)
    conn.commit()  # you must call commit() to persist your data if you don't set autocommit to True
    conn.close()


def main():
    dbname="te1"
    #dbname = getQueryArgument('experiment')
    #dbname = 'tictactoe'
    #dbSession = elixirConnect(credentials, dbname)
    path = 'C:/Dor/'+dbname+'.log'
    hdlr = logging.FileHandler(path)
    formatter = logging.Formatter('%(asctime)s %(levelname)s %(message)s')
    hdlr.setFormatter(formatter)
    logger.addHandler(hdlr)
    logger.setLevel(logging.INFO)
    logger.info("dddd")
    logdb()
    logger.info("DOR Amir")
    try:
        requestType = getQueryArgument('reqType')
        if requestType == "logEvent":
            logEvent()
        else:
            abort400("unknown request type: " + requestType)

        #session.commit()

    except:
        #if anything goes wrong, roll back the database to the previous state
        #dbSession.rollback()
        logger.error(sys.exc_info())
        print 'Conent-Type: text/plain\n'
        print sys.exc_info()
        raise

if __name__ == '__main__':
    main()
