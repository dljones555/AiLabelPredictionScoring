# AiLabelPredictionScoring

Note: to explore this, I used OpenAI ChatGPT to prompt and direction generation of this project. 

Explanation of Metrics:

##Precision

Formula: **Precision = True Positives / (True Positives + False Positives)**
Definition: Precision tells us how many of the items labeled as "taco" (our positive class) by the model were actually tacos. It answers the question, "Out of all the items the model predicted as tacos, how many were correct?"

##Recall (Sensitivity):

Formula: **Recall = True Positives / (True Positives + False Negatives)**
Definition: Recall (also known as Sensitivity) measures the model's ability to find all true tacos. It answers the question, "Out of all the actual tacos in the data, how many did the model correctly identify?

##F1 Score:

Formula: **F1 Score = 2 * (Precision * Recall) / (Precision + Recall)**
Definition: The F1 Score provides a balanced measure that considers both Precision and Recall. It is particularly useful when the data set is imbalanced (i.e., when the number of tacos is much higher or lower than the number of non-tacos), and we want a single score that reflects both accuracy and completeness.

##Purpose of These Metrics:

Precision is important when we want to minimize false positives. For example, in a medical context, you might want to ensure that only people who are truly sick are diagnosed with a certain disease.

Recall is critical when missing positive cases is costly. For example, in fraud detection, you might want to catch as many fraudulent transactions as possible, even if you incorrectly flag some legitimate transactions.

F1 Score helps when there is a need to balance Precision and Recall, especially in cases of imbalanced datasets. A high F1 Score indicates that both Precision and Recall are reasonably high.

These comments clarify how the metrics are calculated and what they represent in the context of prediction tasks.
