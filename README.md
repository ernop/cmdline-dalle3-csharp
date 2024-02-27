# What is this? 

 * command line tool for asynchronously creating tons of images with your OpenAI key via Dalle3, based on a prompt you enter
 * optional prompt syntax for quickly generating tons of variations of a prompt (use {A,,B,C...} and exactly one of the inner items will be chosen. Use [A, B, C] and a subset will be chosen, from 0,1,..all 3 of them, in random order. 
 * automatic saving of raw and annotated images to easily share both your input prompt and the test got revised it to before sending to Dalle3.

## Examples 
"-20 A {naughty, obedient} cat"
this will generate 20 images from that prompt, each time random sending either a naughty cat or an obedient cat.

"-hd -h -10" will generate hd horizontal images. 

## Details 
welcome. this is a command line c sharp application which you can use to communicate with Open AIs image creation endpoint in a more large scale way. it features the ability to control and monitor how many times you submit a prompt the failure or block rate of your attempts, and the ability to easily explore latent space by submitting the permutations which will be explored for you automatically in different combinations and also to submit part of your prompt as a power set. that is, the program will gradually explore all possible subsets of the items in that section of the prompt. this is useful for breaking the GPT layer of the input prompt conversion process out of ruts on getting it to expand and explore more unique possibilities.

the program also contains a few built-in aliases for things like interesting art styles or compositions or locations. for example, instead of doing" a cute little cat" you can do" a cute little cat. {GPTArtstyles}. That is an alias for approximately 500 different types of visual art from simple on such as oil, painting, abstract, watercolor, Byzantine, Mosaic, art, onto more obscure ones like Lucy sombres and aromatic smudged Japanese street, calligraphy; a different one will be chosen for each image. 

You would submit that prompt on the command line, or you'd edit the file called OverridePrompr.cs and then run the program. the UI displays  but your queries as they're made asynchronously , speed depending on the tier which you've reached at OpenAI which is what controls the rate limit that they'll impose on you. as your requests are accepted, the UI will update you successfully saved images as well as blocked, rejected or failed images. breakdowns here go to the individually selected option so you can wll if one word is blocked more than another. 

###Annotation

we save three versions of the image Dalle3 makes:
 * the original
 * original plus rewritten prompt
 * original plus your prompt plus rewritten

### Next version

* refactoring stuff, mainly fixing reporting and progress of how things work
* splitting out a bunch of stuff from program.cs
* mainly about reporting progress as you go, preparing for things like "if a prompt section has failed 100% of the time on a certain option, even after trying like 10 times, you should probably just quit that one."
  * and even more cool stuff later like iteratively adjusting prompts.
  * i.e. initial prompt => 10 images => gptvision them => extract phrases from that into original => iterate
  * and automatically picking up on old threads
  * kind of like there is a huge stack of frames and at each frame you can go back, put it to the bottom of the pile, or make descendants? but there has got to be lots of crossover, which will mess this up.
* Can I put in stuff about cost? so if you are going to make a call that's going to cost like more than 20$, get you to confirm? configurable obv

# Updated Usage

```
Format------ 'Dalle3.exe [-N] [-r] [-h|v] [-hd] [prompt]'
Example-------- 'Dalle3.exe A very {big,blue,tall} photo of a [tall,small] [1-2,cat,dog,mouse monster] i the stylue of {GPTArtstyles}'

Explanation of terms:
        N =                     number of times to repeat prompt. Will die if any fail. Prompt can be multiple words with no quotes required, but no newlines.
        -r =                    output items in random order. default or missing, will output in permutation order. This applies to both permutations and powersets. So without -r you will iterate through all subsets in order.
        -h|v =                  make image horizontal or vertical. the default is square.
        -hd =                   make image in hd. The default is standard, and is cheaper.
        {X,Y,Z,...} =           pick one of these and run with it, just one.
        [X,Y,Z,...] =           powerset operator. two ways to use it: either the first item is in the form A-B where it will pick between A and B items from the list,
        {GPTArtstyles} =        this will pick one of the ~450 artstyles hardcoded into the program. There are a ton of them from all over the world. The program has LOTS of aliases built in
for all kinds of different things. These are useful for forcing the program to get out of the normal vectors. But there are SO many ways to go, I'm still finding out more and more.
        Prompt =                Your text input from the command line. Or, you can edit the file OverridePrompt and run it that way so you can debug, step through etc.
or if you omit that, like in [tall,small], it will pick a random element of the powerset. reminder: powerset means ALL subsets, so everything from none of the items, to 1, to 2, ... to all of them.
Note that for powersets that is a LOT of images. 2^N where N is the number of items in the powerset. Also this is broken right now...
 by default outputs normal and annotated versions of images. If you want no normal do '-nonormal', if you want no annotated do '-noann'```

# Todos
* don't re-send repeatedly blocked prompts upstream, but make this configurable
* show you how much each run is going to cost, including -h -hd etc.
* dynamically avoid words/phrases/expansions which tend to be blocked
* figure out if you are charged for requests which end up getting blocked, and if it varies by which stage you are blocked at?

# Version 1.1 2024.02.10

redoing the way we calculate permutations, etc.

```{a,b,c} => this is a normal permutation guy.
{a boy} => this is really more like an abbreviation for a long description like "a boy with a hound dog puppy, holding a strand of wheat and looking into the distance"
{a boy, a horse} => a permutation prompt section with abbreviations in it.
[a, b, c] => this is a powerset guy (and also probably needs embedded min/maxes, too. hmm) Am I going the right direction here, 
making crazy complex command line control mechanisms?

okay, the problem is that I have both {a} => "longer version of A"
AND I have {a} => {b,c,d}
AND I have {b,c,d} => {"Longer a","longer b",..} and it's not clear what priority they should be being done in.

# cmdline-dalle3-csharp

Two functions:
===

It can conveniently contact dalle3 and make images for you on the command line.

It can take options like --N to make multiple at a time.

It can also expand permutations like "A {big, blue} {cat, dog}" => ["A big cat", "a big dog", "a blue cat", "a blue dog"]. This obviously can generate a LOT of images.

Secondly
===

I'm using it to generate images for custom versions of the SET game where data on the card's four varying aspects is conveyed via other methods than the traditional shape, number, color shading. In this case, by subject, location, plot, and art style, of images generated by dalle3. It's super fun! Fingers crossed that it actually results in something playable.

Some sample images: https://photos.app.goo.gl/4MMt4ZCRV2uujtcp9

Usage
-------

```

"Dalle3.exe [-N] [-r] [-h|v] [-hd] [prompt]

> dalle3.exe A very {big,blue,tall} photo of a {tall,small} {cat,dog,mouse monster}

N=number of times to repeat prompt. Will die if any fail. Prompt can be multiple words with no quotes required, but no newlines." +
{}=run all permutations of the items within here. This can blow up your api limits.
-r output items in random order. default or missing, will output in permutation order.
by default outputs normal and annotated versions of images. If you want no normal do '-nonormal', if you want no annotated do '-noann'
-h|v make image horizontal or vertical. the default is square.
-hd make image in hd. The default is standard, and is cheaper.

```
