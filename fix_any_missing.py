#this is for cleaning up generated images and moving them to archive volders.
#its necessary because sometimes you move one of the sub-files out, and its very annoying to have to re-move afte reevaluating the original or others.
#so, when you're filtering, just remove any variant of the 3 generated files, then run this, and any other incomplete sets will be cleaned.

import os, shutil, datetime
from datetime import datetime, timezone

orig_base="d:/proj/dalle3/output/"

datas=[('annotated','_annotated.png'),('revised','_revised.png'),('', '.png')]
archive_base="d:/proj/dalle3/output/efef/eval/done/"

#procedure:
# collect all inputs with full FP and type
# group them by "save" filename
# if they hvae just 1, move them all to done
# if 2 and are missing revised and are after *date revised was added* then they are done, move them
# if they only have orig, ann, they are not evaluated. So keep them there.

allfps=set()
for dir in datas:
    dirpath=os.path.join(orig_base, dir[0])
    these=os.listdir(dirpath)
    for th in these:
        fp=os.path.join(dirpath, th)
        if os.path.isfile(fp):
            allfps.add(fp)

print("okay, we have loaded all images. There are: %d"%len(allfps))

#background: I have 3 folders, the orig an two descendants for annotated and revised.
#while reviewing, I may have moved a file out from any of those folders into a centralized location.
#if I have done this, I want to grab the other two related images and move them, too.

totalmove=0

def bump():
    global totalmove
    totalmove=totalmove+1

def domov(src, dest):
    global totalmove
    src=src.replace('\\','/')
    dest=dest.replace('\\','/')
    if '\\' in src:
        print("src has.")
    else:
        pass
        #~ print('src oky')
    if '\\' in dest:
        print("desc has.")
    else:
        pass
        #~ print('desc oky')

    bump()
    print("%d %s => %s"%(totalmove, src, dest))
    shutil.move(src, dest)

    return

while True:
    if (len(allfps))==0:
        break

    el=allfps.pop().replace('\\','/')
    if not os.path.exists(el):
        print("already done: %s"%el)
        continue
    #~ import ipdb;ipdb.set_trace()
    hasbase=False
    hasann=False
    hasrev=False

    if "/revised/" in el:
        origfp=el.replace("/revised/","/").replace("_revised.png",".png")
        annfp=el.replace("/revised/","/annotated/").replace("_revised.png","_annotated.png")
        revfp=el
    elif "/annotated/" in el:
        origfp=el.replace("/annotated/","/").replace("_annotated.png",".png")
        annfp=el
        revfp=el.replace("/annotated/","/revised/").replace("_annotated.png","_revised.png")
    else: #this is orig
        origfp=el.replace("/output/","/output/").replace(".png",".png")
        annfp=el.replace("/output/","/output/annotated/").replace(".png","_annotated.png")
        revfp=el.replace("/output/","/output/revised/").replace(".png","_revised.png")

    #~ import ipdb;ipdb.set_trace()
    if '\\' in annfp or '\\' in origfp or '\\' in revfp:
        import ipdb;ipdb.set_trace()
    o_archive_fp=origfp.replace(orig_base, archive_base)
    a_archive_fp=annfp.replace(orig_base+"annotated/", archive_base)
    r_archive_fp=revfp.replace(orig_base+"revised/", archive_base)

    origexi=os.path.exists(origfp)
    annexi=os.path.exists(annfp)
    revexi=os.path.exists(revfp)

    #if all three exist, then it means I haven't done any evaluation of any verison of it, so leave them.
    if origexi and annexi and revexi:
        continue

    #if I've moved the orig or the annotated.

    isOld=False #todo fix this.
    #revised was added 2/22
    if not revexi:
        tm = os.stat(el).st_ctime
        mod_time = datetime.fromtimestamp(tm, tz=timezone.utc)
        compare_date = datetime(2024, 2, 23, tzinfo=timezone.utc)
        #~ import ipdb;ipdb.set_trace()
        if mod_time<compare_date:
            #~ print("is old.")
            isOld=True
        else:
            isOld=False #and therefore we should have the file
            #~ import ipdb;ipdb.set_trace()
        #that is, if you are an old file, and you find that revexi is missing, it does NOT mean that I have edited this.
    if not origexi or not annexi or (not revexi and not isOld):
        if origexi:
            domov(origfp, o_archive_fp)
        if annexi:
            domov(annfp, a_archive_fp)
        if revexi:
            domov(revfp, r_archive_fp)
        continue

    #if I've the rev is missing, the problem is, this may be before that stage.
    #so, do nothing.
    #~ import ipdb;ipdb.set_trace()

#okay now lets search out any extra/variant copies of the images in the target "save" folder too.

treasure_base="D:/dl/prep/"
for fn in os.listdir(treasure_base):
    fp=os.path.join(treasure_base,fn).replace("\\",'/')
    #~ import ipdb;ipdb.set_trace()
    #these are their locations in the "done" folder, which we may potentially copy over to make the "treasure" set complete, for sharing etc.
    origfp=fp.replace(treasure_base, archive_base).replace("_annotated.png",".png").replace("_revised.png",".png")
    annfp=origfp.replace(".png","_annotated.png")
    revfp=origfp.replace(".png","_revised.png")

    if (os.path.exists(origfp)):
        nfp=origfp.replace(archive_base, treasure_base)
        if not os.path.exists(nfp):
            print("RESTORE original %s=>%s"%(origfp, nfp))
            shutil.copy(origfp,nfp)

    if (os.path.exists(annfp)):
        nfp=annfp.replace(archive_base, treasure_base)
        if not os.path.exists(nfp):
            print("RESTORE annotated %s=>%s"%(origfp, nfp))
            shutil.copy(annfp,nfp)

    if (os.path.exists(revfp)):
        nfp=revfp.replace(archive_base, treasure_base)
        if not os.path.exists(nfp):
            print("RESTORE revised %s=>%s"%(origfp, nfp))
            shutil.copy(revfp,nfp)

import sys
sys.exit(0)

ii=0
for file in os.listdir('../output'):
    fp="../output/"+file
    if os.path.isdir(fp):
        continue
    if not file.endswith("_annotated.png"):
        target="../output/efef/eval/"+file
        if os.path.exists(target):
            print("removing:",fp)
            os.remove(fp)
        print("from",fp,"to",target)
        shutil.move(fp, target)
        ii=ii+1
        if ii>20000:
            break

