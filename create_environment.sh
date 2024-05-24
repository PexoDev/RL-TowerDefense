#!/bin/bash

if ! command -v conda &> /dev/null
then
    echo "Conda could not be found. Please install Anaconda or Miniconda and try again."
    exit
fi

ENV_NAME="TowerDefense-RL"
PYTHON_VERSION="3.9.13"

CONDA_REQUIREMENTS="conda_requirements.txt"
PIP_REQUIREMENTS="pip_requirements.txt"

conda create --name $ENV_NAME python=$PYTHON_VERSION -y

if [[ "$OSTYPE" == "msys" ]]; then
    eval "$(conda shell.bash hook)"
    conda activate $ENV_NAME
else
    source $(conda info --base)/etc/profile.d/conda.sh
    conda activate $ENV_NAME
fi

echo "Installing conda packages from $CONDA_REQUIREMENTS..."
while IFS= read -r package
do
    conda install --name $ENV_NAME $package -y
done < $CONDA_REQUIREMENTS

echo "Installing pip packages from $PIP_REQUIREMENTS..."
while IFS= read -r package
do
    pip install $package
done < $PIP_REQUIREMENTS

echo "Environment '$ENV_NAME' created successfully with the specified dependencies."
conda deactivate