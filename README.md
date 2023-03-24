# Science Birds Evaluator

This is a modified version of the [Science Birds](https://github.com/lucasnfe/science-birds) game called Science Birds Evaluator. In this version, the foundational blocks have been adjusted to match with the grid specification of the competition, and a plugin has been added to assess the stability and produce an image for similarity test. The plugin can be accessed via the menu bar by selecting ICE > Batched > Stability Test.

## Getting Started

To get started with this modified version of Science Birds, you'll need to have Unity installed on your computer. Once you have Unity installed, you can clone this repository by running the following command in your terminal:

```
git clone https://github.com/chatgpt4pcg/modified-science-birds.git
```

Once you've cloned the repository, you can open the project in Unity by selecting "Open Project" in the Unity Hub and selecting the `modified-science-birds` folder.

## Usage

To use the Stability Test plugin, simply select "ICE" in the menu bar, choose "Batched", and select "Stability Test". 


This will start the stability testing process immediately. The plugin assumes that the folder structure must follows the following format:

```
<SOURCE_FOLDER>
├── <TEAM_NAME>
|   ├── <STAGE>
│   │    └── <CHARACTER>
│   │       ├── <TRIAL_NUMBER>.jpg
│   │       ├── <TRIAL_NUMBER>.jpg
│   │       └── <TRIAL_NUMBER>.png
│   └── <STAGE>
│        └── <CHARACTER>
│           ├── <TRIAL_NUMBER>.txt
│           ├── <TRIAL_NUMBER>.txt
│           └── <TRIAL_NUMBER>.txt
└── <TEAM_NAME>
    ├── <STAGE>
    │    └── <CHARACTER>
    │       ├── <TRIAL_NUMBER>.jpg
    │       ├── <TRIAL_NUMBER>.png
    │       └── <TRIAL_NUMBER>.jpg
    └── <STAGE>
         └── <CHARACTER>
            ├── <TRIAL_NUMBER>.txt
            ├── <TRIAL_NUMBER>.txt
            └── <TRIAL_NUMBER>.txt
```