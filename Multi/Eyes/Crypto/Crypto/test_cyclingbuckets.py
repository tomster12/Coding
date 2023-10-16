
import random
import matplotlib.pyplot as plt
from locallib import crypto

# Set seed and initialize data
random.seed(1)
pt_msgs_raw = [
    "aThe FitnessGram Pacer TestTestTest is a multistage aerobic capacity test that is a multistage aerobic gets more difficult as it continues",
    "bThe FitnessGram Pacer getsTestmore is a multistage Student exam that progressive is a multistage Student exam first get ready and then start running",
    "cThe FitnessGram Pacer Line up at the start is a multistage childrent exam that teachers do to annoy is a multistage childrent exam why did you forget your pants at home",
    "dThis Fit people test will begin in thirty seconds The running speed starts slowly but gets faster each minute after you hear this signal",
    "eThis Fitness people test will begin in thirty seconds so Line up at the start The running speed starts slowly but dont get fooled cuz it gets faster each minute after you hear this signal",
    "fThis Fit people tests could begin in thirty seconds nineteen dollar fortnite cards The running speed starts slowly who wants them and yes im giving them away",
    "gThis Fitness is the secsonds time you fail a lap before the sound signal Remember to run in a straight line bro The test will begin on the word start On your mark get ready start",
    "hThis Fitn ess is the secsonds or time third time you fail a lap before the so und Remember to run in a straight line today The test will begin on the word start of this mark your test is over",
    "jTh is F it ness is the secsonds time you fail a lap before the sound aerobic Remember to run in a straight line now The test will begin on the word start mark your test is over"
]

# Parse PT alphabet
pt_msgs = [ msg.lower() for msg in pt_msgs_raw ]
pt_alphabet = set()
for msg in pt_msgs:
    pt_alphabet.update(list(msg))
pt_alphabet = sorted(list(pt_alphabet))

# Construct CT buckets and alphabet
current_bucket_end = 0
buckets = {}
for l in pt_alphabet:
    size = random.randrange(2,4)
    buckets[l] = [current_bucket_end, current_bucket_end + size, -1]
    current_bucket_end += size
ct_alphabet = list(range(current_bucket_end))
random.shuffle(ct_alphabet)

# Encode PT to CT
# For each PT letter:
#   - Take current bucket letter
#   - Cycle bucket
#   - Cycle alphabet
def encode(pt, buckets, ct_alphabet):
    for l in buckets: buckets[l][2] = buckets[l][0]
    current_ct_alphabet = ct_alphabet.copy()
    ct_msg = []
    for l in pt:
        # Add ciphertext
        ct_msg.append(current_ct_alphabet[buckets[l][2]])

        # Cycle bucket
        buckets[l][2] -= 1
        if buckets[l][2] < buckets[l][0]:
            buckets[l][2] = buckets[l][1] - 1

        # Cycle alphabet
        current_ct_alphabet = current_ct_alphabet[1:] + [current_ct_alphabet[0]]
    return ct_msg

# Encode all messages
ct_msgs = [ encode(pt, buckets, ct_alphabet) for pt in pt_msgs ]

# Full overview
# crypto.full_overview(ct_msgs)

# Gap frequency
# crypto.plot_gap_freq(ct_msgs)
# plt.show()

# Straight messages
crypto.plot_im(
    im=crypto.conv_msgs_to_im(ct_msgs),
    to_label=True,
    title="Messages",
    figsize=(50,2.4))

# Shared sections
shared_im = crypto.calc_shared(
    msgs=ct_msgs,
    to_zero=True)
crypto.plot_im(
    im=shared_im,
    to_label=True,
    labels=ct_msgs,
    title="Shared Sections",
    to_dull=True,
    under_value=1,
    figsize=(50,2.4))

# Gaps
gap_im = crypto.calc_gaps(
    msgs=ct_msgs,
    limit=16,
    include_end=True,
    use_msg_value=False,
    to_zero=True)
crypto.plot_im(
    im=gap_im,
    to_label=True,
    labels=ct_msgs,
    title="Gaps",
    to_dull=True,
    figsize=((50,2.4)))
crypto.plot_im(
    im=gap_im,
    to_label=True,
    title="Gaps (Sizes)",
    to_dull=True,
    figsize=((50,2.4)))

# Isomorphs
# msgs = ct_msgs[0:9]
msgs = ct_msgs[0:3]
# msgs = ct_msgs[3:6]
# msgs = ct_msgs[6:9]
isos = crypto.calc_isomorphs(msgs)
isos_im = crypto.conv_isomorphs_to_img(msgs, isos, 0)
crypto.plot_im(
    im=isos_im,
    to_label=True,
    to_dull=True,
    labels=msgs,
    title="Isomorphs",
    figsize=((28,1.6)))

plt.show()
