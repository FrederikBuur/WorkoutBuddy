import "./Icon.css";
import { Component, JSX } from "solid-js";

interface IconProps {
  src: Icons;
  onClick: () => void;
  alt?: string;
}

const Icon: Component<IconProps> = (props) => {
  return (
    <div class="circle-icon-container" onClick={props.onClick}>
      <img src={props.src} alt={props.alt ?? "Image"} class="circle-icon" />
    </div>
  );
};

export enum Icons {
  PROFILE = "src/assets/icons/icon_profile.png",
}

export default Icon;
